using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.EntitiesGroupsSpawner;
using FrostfallSaga.Kingdom.InterestPoints;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    ///     Responsible for managing entities movements and encounters.
    /// </summary>
    public class KingdomManager : MonoBehaviour
    {
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public KingdomHexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public List<InterestPoint> InterestPoints { get; private set; }
        [field: SerializeField] public List<EntitiesGroup> EnemiesGroups { get; private set; } = new();
        [SerializeField] private EntitiesGroupsSpawner.EntitiesGroupsSpawner enemiesGroupSpawner;
        [SerializeField] private KingdomLoader kingdomLoader;
        private MovePath _currentHeroGroupMovePath;
        private bool _entitiesAreMoving;

        private EntitiesGroupsMovementController _entitiesGroupsMovementController;
        private bool _mousLeftButtonHold;

        // Parameters are: Encountered enemies group, hero group initiating ?
        public Action<EntitiesGroup, bool> OnEnemiesGroupEncountered;
        public Action<AInterestPointConfigurationSO> OnInterestPointEncountered;

        private void OnKingdomLoaded()
        {
            Setup();
        }

        public void SaveKingdomState()
        {
            EntitiesGroupBuilder entitiesGroupBuilder = EntitiesGroupBuilder.Instance;

            // Extracting Hero Data
            EntitiesGroupData heroGroupData = entitiesGroupBuilder.ExtractEntitiesGroupDataFromEntiesGroup(HeroGroup);

            // Extracting data from enemy groups
            EntitiesGroupData[] enemiesGroupsData = EnemiesGroups
                .Select(group => entitiesGroupBuilder.ExtractEntitiesGroupDataFromEntiesGroup(group))
                .ToArray();

            // Extracting data from points of interest
            InterestPointData[] interestPointsData = InterestPoints
                .Select(point => InterestPointBuilder.ExtractInterestPointData(point))
                .ToArray();

            // Save into GameStateManager
            GameStateManager.Instance.SaveKingdomState(heroGroupData, enemiesGroupsData, interestPointsData);
        }


        // It's inside this function that the magic happens. It makes the hero group then the enemies move.
        private void OnCellClicked(Cell clickedCell)
        {
            if (_entitiesAreMoving || _currentHeroGroupMovePath.PathLength > HeroGroup.movePoints ||
                !_currentHeroGroupMovePath.DoesNextCellExists()) return;

            _entitiesAreMoving = true;
            ResetShorterPathCellsDefaultMaterial();

            // Ask the movement controller to make the groups do the movements and listen to when it finishes.
            _entitiesGroupsMovementController.MakeHeroGroupThenEnemiesGroupMove(
                _currentHeroGroupMovePath,
                EnemiesGroups.ToArray()
            );
        }

        private void OnKingdomCellOccupierClicked(KingdomCellOccupier clickedOccupier)
        {
            OnCellClicked(clickedOccupier.cell);
        }

        private void OnEnemiesGroupEncounteredDuringMovement(EntitiesGroup encounteredEnemiesGroup,
            bool heroGroupHasInitiated)
        {
            OnEnemiesGroupEncountered?.Invoke(encounteredEnemiesGroup, heroGroupHasInitiated);
        }

        private void InterestPointEncountered(InterestPoint encounteredInterestPoint)
        {
            _entitiesAreMoving = false;
            HeroGroup.GetDisplayedEntity().AnimationController.RestoreDefaultAnimation();
            HeroGroup.GetDisplayedEntity().MovementController.RotateTowardsCell(encounteredInterestPoint.cell);
            OnInterestPointEncountered?.Invoke(encounteredInterestPoint.InterestPointConfiguration);
        }

        private void OnAllEntitiesMoved()
        {
            _entitiesAreMoving = false;
            try
            {
                enemiesGroupSpawner.TrySpawnEntitiesGroup();
            }
            catch (ImpossibleSpawnException)
            {
                Debug.Log("Could not spawn one more enemies group.");
            }
        }

        private void OnEnemiesGroupSpawned(EntitiesGroup spawnedEnemiesGroup)
        {
            EnemiesGroups.Add(spawnedEnemiesGroup);
            spawnedEnemiesGroup.MouseEventsController.OnElementHover += OnKingdomCellOccupierHovered;
            spawnedEnemiesGroup.MouseEventsController.OnElementUnhover += OnKingdomCellOccupierUnhovered;
            spawnedEnemiesGroup.MouseEventsController.OnLeftMouseUp += OnKingdomCellOccupierClicked;
        }

        #region Cells hovering and highlighting

        private void OnCellHovered(Cell hoveredCell)
        {
            if (_entitiesAreMoving || _mousLeftButtonHold) // To prevent the player from spaming movements
                return;

            Cell[] shorterPathToHoveredCell = CellsPathFinding.GetShorterPath(
                KingdomGrid,
                HeroGroup.cell,
                hoveredCell,
                checkLastCell: false
            );
            _currentHeroGroupMovePath = new MovePath(shorterPathToHoveredCell);
            HighlightShorterPathCells();
        }

        private void OnLongClickHold(Cell cell)
        {
            _mousLeftButtonHold = true;
        }

        private void OnLongClick(Cell cell)
        {
            _mousLeftButtonHold = false;
        }

        private void OnKingdomCellOccupierHovered(KingdomCellOccupier hoveredOccupier)
        {
            OnCellHovered(hoveredOccupier.cell);
        }

        private void OnCellUnhovered(Cell hoveredCell)
        {
            ResetShorterPathCellsDefaultMaterial();
        }

        private void OnKingdomCellOccupierUnhovered(KingdomCellOccupier unhoveredOccupier)
        {
            OnCellUnhovered(unhoveredOccupier.cell);
        }

        private void HighlightShorterPathCells()
        {
            int i = 0;
            foreach (Cell cell in _currentHeroGroupMovePath.Path)
            {
                if (i < HeroGroup.movePoints)
                    cell.HighlightController.Highlight(HighlightColor.ACCESSIBLE);
                else
                    cell.HighlightController.Highlight(HighlightColor.INACCESSIBLE);
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (Cell cell in _currentHeroGroupMovePath.Path) cell.HighlightController.ResetToInitialColor();
        }

        #endregion

        #region Setup and tear down

        private void Awake()
        {
            if (enemiesGroupSpawner == null)
                enemiesGroupSpawner = FindObjectOfType<EntitiesGroupsSpawner.EntitiesGroupsSpawner>();
            if (enemiesGroupSpawner == null)
            {
                Debug.LogError("No enemies groups spawner found. Enemies groups will not spawn.");
                return;
            }

            if (kingdomLoader == null) kingdomLoader = FindObjectOfType<KingdomLoader>();
            if (kingdomLoader == null)
            {
                Debug.LogError(
                    "No kingdom loader found. Won't be able to correctly manage entities groups after fight.");
                return;
            }

            enemiesGroupSpawner.OnEntitiesGroupSpawned += OnEnemiesGroupSpawned;
            kingdomLoader.onKingdomLoaded += OnKingdomLoaded;

            HeroTeamQuests.Instance.InitializeQuests(this);
        }

        private void Setup()
        {
            KingdomGrid ??= FindObjectOfType<KingdomHexGrid>();
            if (KingdomGrid == null)
            {
                Debug.LogError("No HexGrid found in the scene. The Kingdom manager can't work.");
                gameObject.SetActive(false);
                return;
            }

            BindCellMouseEvents();

            HeroGroup = FindObjectsOfType<EntitiesGroup>().ToList()
                .Find(entitiesGroup => entitiesGroup.name == "HeroGroup");
            EnemiesGroups = FindObjectsOfType<EntitiesGroup>().ToList()
                .FindAll(entitiesGroup => entitiesGroup.name != "HeroGroup");
            InterestPoints = FindObjectsOfType<InterestPoint>().ToList();
            _entitiesGroupsMovementController = new EntitiesGroupsMovementController(KingdomGrid, HeroGroup);
            _entitiesGroupsMovementController.OnAllEntitiesMoved += OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered += OnEnemiesGroupEncounteredDuringMovement;
            _entitiesGroupsMovementController.OnInterestPointEncountered += InterestPointEncountered;

            BindOccupiersMouseEvents();
        }

        private void BindCellMouseEvents()
        {
            foreach (Cell cell in KingdomGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
                cell.CellMouseEventsController.OnLongClickHold += OnLongClickHold;
                cell.CellMouseEventsController.OnLongClick += OnLongClick;
            }
        }

        private void BindOccupiersMouseEvents()
        {
            KingdomCellOccupier[] occupiers = FindObjectsOfType<KingdomCellOccupier>();
            foreach (KingdomCellOccupier occupier in occupiers)
            {
                occupier.MouseEventsController.OnElementHover += OnKingdomCellOccupierHovered;
                occupier.MouseEventsController.OnElementUnhover += OnKingdomCellOccupierUnhovered;
                occupier.MouseEventsController.OnLeftMouseUp += OnKingdomCellOccupierClicked;
            }
        }

        #endregion
    }
}