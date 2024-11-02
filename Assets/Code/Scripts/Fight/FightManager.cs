using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Controllers;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Fight.UI;

namespace FrostfallSaga.Fight
{
    /// <summary>
    /// Controls the proceedings of a fight between two teams of fighters.
    /// </summary>
    public class FightManager : MonoBehaviour
    {
        public Action<Fighter, bool> onFighterTurnBegan; // <the fighter that plays, if he's an ally>
        public Action<Fighter, bool> onFighterTurnEnded; // <the fighter that plays, if he's an ally>
        public Action<Fighter[]> onFightersTurnOrderUpdated;
        public Action<Fighter[], Fighter[]> onFightEnded;   // <allies, enemies>

        // Needed external components
        [SerializeField] private FightersGenerator _fightersGenerator;
        [SerializeField] private HexGrid _fightGrid;
        [SerializeField] private FighterActionPanelController _actionPanel;
        [SerializeField] private Material _cellHighlightMaterial;
        [SerializeField] private Material _cellActionableHighlightMaterial;
        [SerializeField] private Material _cellInaccessibleHighlightMateria;

        // Controllers
        private PlayerController _playerController;
        private FBTController _fbtController;
        private RandomController _randomController;

        private List<Fighter> _allies;
        private List<Fighter> _enemies;
        private Queue<Fighter> _fightersTurnOrder;
        private Fighter[] _initialFightersTurnOrder;
        private Fighter _playingFighter;

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            _playingFighter = null;
            _allies = new(allies);
            _enemies = new(enemies);
            SubscribeToFightersEvents();
            PositionFightersOnGrid(_fightGrid, _allies.ToArray(), _enemies.ToArray());
            UpdateFightersTurnOrder(GetFightersTurnOrder(_allies.Concat(_enemies).ToArray()));
            PlayNextFighterTurn();
        }

        private void PlayNextFighterTurn()
        {
            _playingFighter = _fightersTurnOrder.Dequeue();
            _playingFighter.StatusesManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            AFighterController controller = GetControllerForFighter(_playingFighter);
            onFighterTurnBegan(_playingFighter, _allies.Contains(_playingFighter));
            controller.PlayTurn(_playingFighter, GetFighterTeamsAsDict(_allies, _enemies), _fightGrid);
        }

        private AFighterController GetControllerForFighter(Fighter fighter)
        {
            if (_allies.Contains(fighter))
            {
                return _playerController;
            }
            if (fighter.PersonalityTrait == null)
            {
                return _randomController;
            }
            return _fbtController;
        }

        private void OnFighterTurnEnded(Fighter fighterThatPlayed)
        {
            if (HasFightEnded())    // All fight ended events already handled in OnFighterActionEnded
            {
                return;
            }

            fighterThatPlayed.ResetMovementAndActionPoints();
            fighterThatPlayed.StatusesManager.UpdateStatuses(EStatusTriggerTime.EndOfTurn);
            _fightersTurnOrder.Enqueue(fighterThatPlayed);
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());
            PlayNextFighterTurn();
        }

        private void OnFighterActionEnded(Fighter fighterThatAct)
        {
            if (HasFightEnded())
            {
                Debug.Log($"Winner is {GetWinner(_allies, _enemies)}");
                onFightEnded?.Invoke(_allies.ToArray(), _enemies.ToArray());
                return;
            }

            Queue<Fighter> updatedFighterTurnsOrder = GetFightersTurnOrder(_allies.Concat(_enemies).ToArray());
            if (!CompareFighterTurnOrder(updatedFighterTurnsOrder.ToArray(), _initialFightersTurnOrder))
            {
                UpdateFightersTurnOrder(updatedFighterTurnsOrder);
            }
        }

        private void OnFighterDied(Fighter fighterThatDied)
        {
            // Deactivate dead fighter
            fighterThatDied.gameObject.SetActive(false);
            fighterThatDied.cell.SetFighter(null);

            // Update order
            Queue<Fighter> updatedFighterTurnsOrder = GetFightersTurnOrder(_allies.Concat(_enemies).ToArray());
            if (!CompareFighterTurnOrder(updatedFighterTurnsOrder.ToArray(), _initialFightersTurnOrder))
            {
                UpdateFightersTurnOrder(updatedFighterTurnsOrder);
            }

            // If suicide, end fight if needed, otherwise go to next fight
            if (fighterThatDied == _playingFighter)
            {
                if (HasFightEnded())
                {
                    Debug.Log($"Winner is {GetWinner(_allies, _enemies)}");
                    onFightEnded?.Invoke(_allies.ToArray(), _enemies.ToArray());
                    return;
                }
                PlayNextFighterTurn();
            }
        }

        private void UpdateFightersTurnOrder(Queue<Fighter> newFightersTurnOrder)
        {
            _fightersTurnOrder = newFightersTurnOrder;
            _initialFightersTurnOrder = newFightersTurnOrder.ToArray();
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());
        }

        private bool HasFightEnded()
        {
            return GetWinner(_allies, _enemies) != EWinner.NO_ONE;
        }

        #region Fight manager components setup and tear down
        private void SubscribeToFightersEvents()
        {
            _allies.Union(_enemies).ToList().ForEach(fighter => fighter.onFighterDied += OnFighterDied);
        }

        private void OnEnable()
        {
            if (_fightGrid == null)
            {
                _fightGrid = FindObjectOfType<HexGrid>();
            }
            if (_fightGrid == null)
            {
                Debug.LogError("No fight grid found.");
                return;
            }

            if (_fightersGenerator == null)
            {
                _fightersGenerator = FindObjectOfType<FightersGenerator>();
            }
            if (_fightersGenerator == null)
            {
                Debug.LogError("No fighters generator found. Can't start fight.");
            }

            // Setup controllers
            _playerController = gameObject.AddComponent<PlayerController>();
            _playerController.Setup(
                _actionPanel,
                _cellHighlightMaterial,
                _cellActionableHighlightMaterial,
                _cellInaccessibleHighlightMateria

            );
            _playerController.onFighterTurnEnded += OnFighterTurnEnded;
            _playerController.onFighterActionEnded += OnFighterActionEnded;

            _fbtController = gameObject.AddComponent<FBTController>();
            _fbtController.Setup();
            _fbtController.onFighterTurnEnded += OnFighterTurnEnded;
            _fbtController.onFighterActionEnded += OnFighterActionEnded;

            _randomController = gameObject.AddComponent<RandomController>();
            _randomController.Setup();
            _randomController.onFighterTurnEnded += OnFighterTurnEnded;
            _randomController.onFighterActionEnded += OnFighterActionEnded;

            _fightersGenerator.onFightersGenerated += OnFightersGenerated;
        }

        private void OnDisable()
        {
            if (_fightersGenerator != null)
            {
                _fightersGenerator.onFightersGenerated -= OnFightersGenerated;
            }

            if (_playerController != null)
            {
                _playerController.onFighterTurnEnded -= OnFighterTurnEnded;
                _playerController.onFighterActionEnded += OnFighterActionEnded;
            }
            if (_fbtController != null)
            {
                _fbtController.onFighterTurnEnded -= OnFighterTurnEnded;
                _fbtController.onFighterActionEnded -= OnFighterActionEnded;
            }
            if (_randomController != null)
            {
                _randomController.onFighterTurnEnded -= OnFighterTurnEnded;
                _randomController.onFighterActionEnded -= OnFighterActionEnded;
            }
        }
        #endregion

        #region Static helper methods
        private static Dictionary<Fighter, bool> GetFighterTeamsAsDict(List<Fighter> allies, List<Fighter> enemies)
        {
            Dictionary<Fighter, bool> teams = new();
            allies.ForEach(ally => teams.Add(ally, true));
            enemies.ForEach(enemy => teams.Add(enemy, false));
            return teams;
        }

        private static void PositionFightersOnGrid(HexGrid fightGrid, Fighter[] allies, Fighter[] enemies)
        {
            int xCellIndex = 0;
            foreach (Fighter ally in allies)
            {
                FightCell cellForAlly = (FightCell)fightGrid.CellsByCoordinates[new(xCellIndex, 0)];
                cellForAlly.SetFighter(ally);
                ally.cell = cellForAlly;
                ally.MovementController.RotateTowardsCell(fightGrid.CellsByCoordinates[new(xCellIndex, 1)]);
                ally.MovementController.TeleportToCell(cellForAlly);
                xCellIndex++;
            }

            xCellIndex = 0;
            foreach (Fighter enemy in enemies)
            {
                FightCell cellForEnemy = (FightCell)fightGrid.CellsByCoordinates[new(xCellIndex, fightGrid.Height - 1)];
                cellForEnemy.SetFighter(enemy);
                enemy.cell = cellForEnemy;
                enemy.MovementController.RotateTowardsCell(fightGrid.CellsByCoordinates[new(xCellIndex, fightGrid.Height - 2)]);
                enemy.MovementController.TeleportToCell(cellForEnemy);
                xCellIndex++;
            }
        }

        private static EWinner GetWinner(List<Fighter> allies, List<Fighter> enemies)
        {
            int teamHealth = 0;
            allies.ForEach(ally => teamHealth += ally.GetHealth());
            if (teamHealth <= 0)  // Should be equal to zero at minimum, but just in case, we check for negative values
            {
                return EWinner.ENEMIES;
            }

            teamHealth = 0;
            enemies.ForEach(enemy => teamHealth += enemy.GetHealth());
            if (teamHealth <= 0)  // Should be equal to zero at minimum, but just in case, we check for negative values
            {
                return EWinner.ALLIES;
            }

            return EWinner.NO_ONE;
        }

        private static Queue<Fighter> GetFightersTurnOrder(Fighter[] fighters)
        {
            return new(
                fighters
                    .Where(fighter => fighter.GetHealth() > 0)
                    .OrderByDescending(fighter => fighter.GetInitiative())
            );
        }

        private static bool CompareFighterTurnOrder(Fighter[] order1, Fighter[] order2)
        {
            if (order1.Length != order2.Length)
            {
                return false;
            }

            for (int i = 0; i < order1.Length; i++)
            {
                if (order1[i] != order2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        private enum EWinner
        {
            ALLIES,
            ENEMIES,
            NO_ONE
        }
    }
}