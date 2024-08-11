using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Controllers;
using FrostfallSaga.Grid.Cells;

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
        public Action<bool> onFightEnded;   // If the allies won or not

        [SerializeField] private FightersGenerator _fightersGenerator;
        [SerializeField] private HexGrid _fightGrid;
        [SerializeField] private AFighterController _alliesController;
        [SerializeField] private AFighterController _enemiesController;

        private Fighter[] _allies;
        private Fighter[] _enemies;
        private Queue<Fighter> _fightersTurnOrder;
        private Fighter[] _initialFightersTurnOrder;

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            _allies = allies;
            _enemies = enemies;
            PositionFightersOnGrid(_fightGrid, _allies, _enemies);
            UpdateFightersTurnOrder(GetFightersTurnOrder(_allies.Concat(_enemies).ToArray()));
            PlayNextFighterTurn();
        }

        private void PlayNextFighterTurn()
        {
            Fighter fighterToPlay = _fightersTurnOrder.Dequeue();
            bool isAlly = _allies.Contains(fighterToPlay);
            AFighterController controller = isAlly ? _alliesController : _enemiesController;
            onFighterTurnBegan(fighterToPlay, isAlly);
            controller.PlayTurn(fighterToPlay, GetFighterTeamsAsDict(_allies, _enemies), _fightGrid);
        }

        private void OnFighterTurnEnded(Fighter fighterThatPlayed)
        {
            if (CheckForFightEnd())
            {
                Debug.Log($"Winner is {GetWinner(_allies, _enemies)}");
                return;
            }

            fighterThatPlayed.ResetMovementAndActionPoints();
            _fightersTurnOrder.Enqueue(fighterThatPlayed);
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());
            PlayNextFighterTurn();
        }

        private void OnFighterActionEnded(Fighter fighterThatAct)
        {
            if (CheckForFightEnd())
            {
                Debug.Log($"Winner is {GetWinner(_allies, _enemies)}");
                return;
            }

            Queue<Fighter> updatedFighterTurnsOrder = GetFightersTurnOrder(_allies.Concat(_enemies).ToArray());
            if (!CompareFighterTurnOrder(updatedFighterTurnsOrder.ToArray(), _initialFightersTurnOrder))
            {
                UpdateFightersTurnOrder(updatedFighterTurnsOrder);
            }
        }

        private void UpdateFightersTurnOrder(Queue<Fighter> newFightersTurnOrder)
        {
            _fightersTurnOrder = newFightersTurnOrder;
            _initialFightersTurnOrder = newFightersTurnOrder.ToArray();
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());
        }

        private bool CheckForFightEnd()
        {
            EWinner possibleWinner = GetWinner(_allies, _enemies);
            if (possibleWinner == EWinner.NO_ONE)
            {
                return false;
            }
            onFightEnded?.Invoke(possibleWinner == EWinner.ALLIES);
            return true;
        }

        #region Fight manager components setup and tear down
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

            if (_alliesController == null)
            {
                Debug.LogError("No controller for allies set.");
                return;
            }
            if (_enemiesController == null)
            {
                Debug.LogError("No controller for enemies set.");
                return;
            }

            _fightersGenerator.onFightersGenerated += OnFightersGenerated;

            _alliesController.onFighterTurnEnded += OnFighterTurnEnded;
            _alliesController.onFighterActionEnded += OnFighterActionEnded;

            _enemiesController.onFighterTurnEnded += OnFighterTurnEnded;
            _enemiesController.onFighterActionEnded += OnFighterActionEnded;
        }

        private void OnDisable()
        {
            if (_fightersGenerator == null)
            {
                _fightersGenerator = FindObjectOfType<FightersGenerator>();
            }
            if (_fightersGenerator == null)
            {
                Debug.LogWarning("No fighters generator found. Can't tear down properly.");
            }

            if (_alliesController == null)
            {
                Debug.LogWarning("No controller for allies set.");
                return;
            }
            if (_enemiesController == null)
            {
                Debug.LogWarning("No controller for enemies set.");
                return;
            }

            _fightersGenerator.onFightersGenerated -= OnFightersGenerated;

            _alliesController.onFighterTurnEnded -= OnFighterTurnEnded;
            _alliesController.onFighterActionEnded += OnFighterActionEnded;

            _enemiesController.onFighterTurnEnded -= OnFighterTurnEnded;
            _enemiesController.onFighterActionEnded -= OnFighterActionEnded;
        }
        #endregion

        #region Static helper methods
        private static Dictionary<Fighter, bool> GetFighterTeamsAsDict(Fighter[] allies, Fighter[] enemies)
        {
            Dictionary<Fighter, bool> teams = new();
            allies.ToList().ForEach(ally => teams.Add(ally, true));
            enemies.ToList().ForEach(enemy => teams.Add(enemy, false));
            return teams;
        }

        private static void PositionFightersOnGrid(HexGrid fightGrid, Fighter[] allies, Fighter[] enemies)
        {
            int xCellIndex = 0;
            foreach (Fighter ally in allies)
            {
                Cell cellForAlly = fightGrid.CellsByCoordinates[new(xCellIndex, 0)];
                cellForAlly.GetComponent<CellFightBehaviour>().Fighter = ally;
                ally.cell = cellForAlly;
                ally.MovementController.RotateTowardsCell(fightGrid.CellsByCoordinates[new(xCellIndex, 1)]);
                ally.MovementController.TeleportToCell(cellForAlly);
                xCellIndex++;
            }

            xCellIndex = 0;
            foreach (Fighter enemy in enemies)
            {
                Cell cellForEnemy = fightGrid.CellsByCoordinates[new(xCellIndex, fightGrid.Height - 1)];
                cellForEnemy.GetComponent<CellFightBehaviour>().Fighter = enemy;
                enemy.cell = cellForEnemy;
                enemy.MovementController.RotateTowardsCell(fightGrid.CellsByCoordinates[new(xCellIndex, fightGrid.Height - 2)]);
                enemy.MovementController.TeleportToCell(cellForEnemy);
                xCellIndex++;
            }
        }

        private static EWinner GetWinner(Fighter[] allies, Fighter[] enemies)
        {
            int teamHealth = 0;
            allies.ToList().ForEach(ally => teamHealth += ally.GetHealth());
            if (teamHealth <= 0)  // Should be equal to zero at minimum, but just in case, we check for negative values
            {
                return EWinner.ENEMIES;
            }

            teamHealth = 0;
            enemies.ToList().ForEach(enemy => teamHealth += enemy.GetHealth());
            if (teamHealth <= 0)  // Should be equal to zero at minimum, but just in case, we check for negative values
            {
                return EWinner.ALLIES;
            }

            return EWinner.NO_ONE;
        }

        private static Queue<Fighter> GetFightersTurnOrder(Fighter[] fighters)
        {
            Debug.Log(fighters.Where(fighter => fighter.GetHealth() > 0).ToArray().Length);
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