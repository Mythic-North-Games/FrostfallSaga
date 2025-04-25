using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Fight.Controllers;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Fight.UI;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

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
            onFighterTurnBegan?.Invoke(_playingFighter, _allies.Contains(_playingFighter));
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

            // Notify end of turn
            onFighterTurnEnded?.Invoke(fighterThatPlayed, _allies.Contains(fighterThatPlayed));

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
                onFighterTurnEnded?.Invoke(fighterThatDied, _allies.Contains(fighterThatDied));
                onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());
                PlayNextFighterTurn();
                return;
            }

            // Notify turn order update
            onFightersTurnOrderUpdated?.Invoke(_fightersTurnOrder.ToArray());

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
                _actionPanel
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
            _fightLoader.onFightLoaded += OnFightLoaded;
        }

        #endregion

        #region Private helper methods

        private static Dictionary<Fighter, bool> GetFighterTeamsAsDict(List<Fighter> allies, List<Fighter> enemies)
        {
            Dictionary<Fighter, bool> teams = new();
            allies.ForEach(ally => teams.Add(ally, true));
            enemies.ForEach(enemy => teams.Add(enemy, false));
            return teams;
        }

        private void PositionFightersOnGrid(FightHexGrid fightGrid, Fighter[] allies, Fighter[] enemies)
        {
            List<FightCell> availableAllyCells = GetStartingCells(fightGrid, isForAllies: true);
            List<FightCell> availableEnemyCells = GetStartingCells(fightGrid, isForAllies: false);

            for (int i = 0; i < allies.Length; i++)
            {
                if (i >= availableAllyCells.Count)
                {
                    Debug.LogWarning($"Not enough accessible cells to place ally {allies[i].name}.");
                    continue;
                }

                PlaceFighterOnCell(allies[i], availableAllyCells[i], facingYDirection: 1);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                if (i >= availableEnemyCells.Count)
                {
                    Debug.LogWarning($"Not enough accessible cells to place enemy {enemies[i].name}.");
                    continue;
                }

                PlaceFighterOnCell(enemies[i], availableEnemyCells[i], facingYDirection: -1);
            }
        }

        private static List<FightCell> GetStartingCells(FightHexGrid grid, bool isForAllies)
        {
            List<FightCell> cells = new();
            int startY = isForAllies ? 0 : grid.Height - 1;

            for (int x = 0; x < grid.Width; x++)
            {
                Vector2Int coord = new(x, startY);
                if (grid.Cells.TryGetValue(coord, out Cell cell) &&
                    cell is FightCell fightCell && fightCell.IsTerrainAccessible() &&
                    fightCell.IsFree())
                {
                    cells.Add(fightCell);
                }
            }

            return cells;
        }

        private void PlaceFighterOnCell(Fighter fighter, FightCell cell, int facingYDirection)
        {
            cell.SetFighter(fighter);
            fighter.cell = cell;

            Vector2Int facingCoord = new(cell.Coordinates.x, cell.Coordinates.y + facingYDirection);
            if (FightGrid.Cells.TryGetValue(facingCoord, out Cell target))
            {
                fighter.MovementController.RotateTowardsCell(target);
            }

            fighter.MovementController.TeleportToCell(cell);
        }


        private static EWinner GetWinner(List<Fighter> allies, List<Fighter> enemies)
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

        private static List<Fighter> GetAbsoluteFightersTurnOrder(Fighter[] fighters)
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