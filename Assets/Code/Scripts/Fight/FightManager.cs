using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Fight.Controllers;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Fight.UI;

namespace FrostfallSaga.Fight
{
    /// <summary>
    ///     Controls the proceedings of a fight between two teams of fighters.
    /// </summary>
    public class FightManager : MonoBehaviour
    {
        [field: SerializeField] public int RoundCount { get; private set; }

        // Needed external components
        [SerializeField] private FightLoader _fightLoader;

        [field: SerializeField] public FightHexGrid FightGrid { get; private set; }
        [SerializeField] private FighterActionPanelController _actionPanel;
        [SerializeField] private Material _cellHighlightMaterial;
        [SerializeField] private Material _cellActionableHighlightMaterial;
        [SerializeField] private Material _cellInaccessibleHighlightMateria;

        // Internal state
        private List<Fighter> _allies;
        private List<Fighter> _enemies;
        private FBTController _fbtController;
        private List<Fighter> _fightersTurnOrder;

        // Controllers
        private PlayerController _playerController;
        private Fighter _playingFighter;
        private RandomController _randomController;
        public Action<Fighter[], Fighter[]> onFightEnded; // <allies, enemies>
        public Action<Fighter[]> onFightersTurnOrderUpdated;

        public Action<Fighter, bool> onFighterTurnBegan; // <the fighter that plays, if he's an ally>
        public Action<Fighter, bool> onFighterTurnEnded; // <the fighter that plays, if he's an ally>

        public Dictionary<Fighter, bool> FighterTeams => GetFighterTeamsAsDict(_allies, _enemies);

        private void Awake()
        {
            HeroTeamQuests.Instance.InitializeQuests(this);
        }

        private void OnFightLoaded(Fighter[] allies, Fighter[] enemies)
        {
            // Init
            _playingFighter = null;
            _allies = new List<Fighter>(allies);
            _enemies = new List<Fighter>(enemies);
            RoundCount = 0;
            SubscribeToFightersEvents();

            // Position fighters on grid
            PositionFightersOnGrid(FightGrid, _allies.ToArray(), _enemies.ToArray());

            // Update fighters turn order
            _fightersTurnOrder = GetAbsoluteFightersTurnOrder(_allies.Concat(_enemies).ToArray());
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());

            // Update fighters passive abilities
            UpdateFightersPassiveAbilities();

            // Launch first turn
            PlayNextFighterTurn();
        }

        private void PlayNextFighterTurn()
        {
            RoundCount++;

            _playingFighter = _fightersTurnOrder[0];
            _playingFighter.StatusesManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            AFighterController controller = GetControllerForFighter(_playingFighter);
            onFighterTurnBegan(_playingFighter, _allies.Contains(_playingFighter));
            controller.PlayTurn(_playingFighter);
        }

        private AFighterController GetControllerForFighter(Fighter fighter)
        {
            if (_allies.Contains(fighter)) return _playerController;
            if (fighter.PersonalityTrait == null) return _randomController;
            return _fbtController;
        }

        private void OnFighterTurnEnded(Fighter fighterThatPlayed)
        {
            if (HasFightEnded()) // All fight ended events already handled in OnFighterActionEnded
                return;

            // Update fighter stats & statuses
            fighterThatPlayed.ResetMovementAndActionPoints();
            fighterThatPlayed.StatusesManager.UpdateStatuses(EStatusTriggerTime.EndOfTurn);

            // Update cells alterations
            UpdateFightGridCellsAlterations();

            // Update turn order
            _fightersTurnOrder.RemoveAt(0);
            _fightersTurnOrder.Add(fighterThatPlayed);
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());

            // Launch next turn
            PlayNextFighterTurn();
        }

        private void OnFighterActionEnded(Fighter fighterThatAct)
        {
            // Check for fight end first
            if (HasFightEnded())
            {
                Debug.Log($"Winner is {GetWinner(_allies, _enemies)}");
                onFightEnded?.Invoke(_allies.ToArray(), _enemies.ToArray());
                return;
            }

            // Update fighter passive abilities
            UpdateFightersPassiveAbilities();
        }

        private void OnFighterDied(Fighter fighterThatDied)
        {
            // Deactivate dead fighter
            fighterThatDied.gameObject.SetActive(false);
            fighterThatDied.cell.SetFighter(null);

            // Update fighters passive abilities because a condition regarding the death could now be met
            UpdateFightersPassiveAbilities();

            // Update order
            _fightersTurnOrder.Remove(fighterThatDied);
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());

            // If suicide, end fight if needed, otherwise end fighter turn
            if (fighterThatDied == _playingFighter)
            {
                if (HasFightEnded())
                {
                    Debug.Log($"Winner is {GetWinner(_allies, _enemies)}");
                    onFightEnded?.Invoke(_allies.ToArray(), _enemies.ToArray());
                    return;
                }

                UpdateFightGridCellsAlterations();
                PlayNextFighterTurn();
                return;
            }

            // Increase god favors points for the fighter that killed the other
            _playingFighter.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.KILL);
        }


        private bool HasFightEnded()
        {
            return GetWinner(_allies, _enemies) != EWinner.NO_ONE;
        }

        public Fighter[] GetMatesOfFighter(Fighter fighter)
        {
            if (_allies.Contains(fighter)) return _allies.Select(ally => ally).Where(ally => ally != fighter).ToArray();
            return _enemies.Select(enemy => enemy).Where(enemy => enemy != fighter).ToArray();
        }

        private enum EWinner
        {
            ALLIES,
            ENEMIES,
            NO_ONE
        }

        #region Fight manager components setup and tear down

        private void SubscribeToFightersEvents()
        {
            _allies.Union(_enemies).ToList().ForEach(fighter => fighter.onFighterDied += OnFighterDied);
        }

        private void OnEnable()
        {
            if (FightGrid == null) FightGrid = FindObjectOfType<FightHexGrid>();
            if (FightGrid == null)
            {
                Debug.LogError("No fight grid found.");
                return;
            }

            if (_fightLoader == null) _fightLoader = FindObjectOfType<FightLoader>();
            if (_fightLoader == null) Debug.LogError("No fighters generator found. Can't start fight.");

            // Setup controllers
            _playerController = gameObject.AddComponent<PlayerController>();
            _playerController.Setup(
                this,
                _actionPanel,
                _cellHighlightMaterial,
                _cellActionableHighlightMaterial,
                _cellInaccessibleHighlightMateria
            );
            _playerController.onFighterTurnEnded += OnFighterTurnEnded;
            _playerController.onFighterActionEnded += OnFighterActionEnded;

            _fbtController = gameObject.AddComponent<FBTController>();
            _fbtController.Setup(this);
            _fbtController.onFighterTurnEnded += OnFighterTurnEnded;
            _fbtController.onFighterActionEnded += OnFighterActionEnded;

            _randomController = gameObject.AddComponent<RandomController>();
            _randomController.Setup(this);
            _randomController.onFighterTurnEnded += OnFighterTurnEnded;
            _randomController.onFighterActionEnded += OnFighterActionEnded;
            _fightLoader.OnFightLoaded += OnFightLoaded;
        }

        #endregion

        #region Private helper methods

        private Dictionary<Fighter, bool> GetFighterTeamsAsDict(List<Fighter> allies, List<Fighter> enemies)
        {
            Dictionary<Fighter, bool> teams = new();
            allies.ForEach(ally => teams.Add(ally, true));
            enemies.ForEach(enemy => teams.Add(enemy, false));
            return teams;
        }

        private void PositionFightersOnGrid(FightHexGrid fightGrid, Fighter[] allies, Fighter[] enemies)
        {
            int xCellIndex = 0;
            foreach (Fighter ally in allies)
            {
                FightCell cellForAlly = (FightCell)fightGrid.Cells[new Vector2Int(xCellIndex, 0)];
                cellForAlly.SetFighter(ally);
                ally.cell = cellForAlly;
                ally.MovementController.RotateTowardsCell(fightGrid.Cells[new Vector2Int(xCellIndex, 1)]);
                ally.MovementController.TeleportToCell(cellForAlly);
                xCellIndex++;
            }

            xCellIndex = 0;
            foreach (Fighter enemy in enemies)
            {
                FightCell cellForEnemy = (FightCell)fightGrid.Cells[new Vector2Int(xCellIndex, fightGrid.Height - 1)];
                cellForEnemy.SetFighter(enemy);
                enemy.cell = cellForEnemy;
                enemy.MovementController.RotateTowardsCell(
                    fightGrid.Cells[new Vector2Int(xCellIndex, fightGrid.Height - 2)]);
                enemy.MovementController.TeleportToCell(cellForEnemy);
                xCellIndex++;
            }
        }

        private EWinner GetWinner(List<Fighter> allies, List<Fighter> enemies)
        {
            int teamHealth = 0;
            allies.ForEach(ally => teamHealth += ally.GetHealth());
            if (teamHealth <= 0) // Should be equal to zero at minimum, but just in case, we check for negative values
                return EWinner.ENEMIES;

            teamHealth = 0;
            enemies.ForEach(enemy => teamHealth += enemy.GetHealth());
            if (teamHealth <= 0) // Should be equal to zero at minimum, but just in case, we check for negative values
                return EWinner.ALLIES;

            return EWinner.NO_ONE;
        }

        private List<Fighter> GetAbsoluteFightersTurnOrder(Fighter[] fighters)
        {
            return new List<Fighter>(
                fighters
                    .Where(fighter => fighter.GetHealth() > 0)
                    .OrderByDescending(fighter => fighter.GetInitiative())
            );
        }

        private void UpdateFightGridCellsAlterations()
        {
            FightCell[] cells = Array.ConvertAll(FightGrid.GetCells(), cell => (FightCell)cell);
            foreach (FightCell cell in cells) cell.UpdateAlterations();
        }

        private void UpdateFightersPassiveAbilities()
        {
            _allies.ForEach(ally => ally.PassiveAbilitiesManager.UpdatePassiveAbilities(FightGrid, FighterTeams));
            _enemies.ForEach(enemy => enemy.PassiveAbilitiesManager.UpdatePassiveAbilities(FightGrid, FighterTeams));
        }

        #endregion
    }
}