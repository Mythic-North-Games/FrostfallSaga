using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.EntitiesGroupsSpawner;
using FrostfallSaga.Kingdom.InterestPoints;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class KingdomManager : MonoBehaviour
    {
        // Parameters are: Encountered enemies group, hero group initiating ?
        public Action<EntitiesGroup, bool> onEnemiesGroupEncountered;
        public Action<AInterestPointConfigurationSO> onInterestPointEncountered;

        [field: SerializeField] public Material CellHighlightMaterial { get; private set; }
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public AHexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public List<EntitiesGroup> EnemiesGroups { get; private set; } = new();
        [field: SerializeField] public InterestPoint[] InterestPoints { get; private set; }

        [SerializeField] private EntitiesGroupsSpawner.EntitiesGroupsSpawner _enemiesGroupSpawner;
        [SerializeField] private KingdomLoader _kingdomLoader;
        private EntitiesGroupsMovementController _entitiesGroupsMovementController;
        private MovePath _currentHeroGroupMovePath;
        private bool _entitiesAreMoving;

        private void OnKingdomLoaded()
        {
            Setup();
        }

        public void SaveKingdomState()
        {
            EntitiesGroupBuilder entitiesGroupBuilder = EntitiesGroupBuilder.Instance;
            EntitiesGroupData heroGroupData = entitiesGroupBuilder.ExtractEntitiesGroupDataFromEntiesGroup(HeroGroup);

            List<EntitiesGroupData> enemiesGroupsData = new();
            EnemiesGroups.ForEach(group =>
            {
                enemiesGroupsData.Add(entitiesGroupBuilder.ExtractEntitiesGroupDataFromEntiesGroup(group));
            });

            List<InterestPointData> interestPointsData = new();
            foreach (InterestPoint interestPoint in InterestPoints)
            {
                interestPointsData.Add(InterestPointBuilder.Instance.ExtractInterestPointDataFromInterestPoint(interestPoint));
            }
            GameStateManager.Instance.SaveKingdomState(heroGroupData, enemiesGroupsData.ToArray(), interestPointsData.ToArray());
            Debug.Log("KingdomConfiguration Saved !");
        }

        // It's inside this function that the magic happens. It makes the hero group then the enemies move.
        private void OnCellClicked(Cell clickedCell)
        {
            if (_entitiesAreMoving || _currentHeroGroupMovePath.PathLength > HeroGroup.movePoints || !_currentHeroGroupMovePath.DoesNextCellExists())
            {
                return;
            }

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

        private void OnEnemiesGroupEncounteredDuringMovement(EntitiesGroup encounteredEnemiesGroup, bool heroGroupHasInitiated)
        {
            onEnemiesGroupEncountered?.Invoke(encounteredEnemiesGroup, heroGroupHasInitiated);
        }

        private void OnInterestPointEncountered(InterestPoint encounteredInterestPoint)
        {
            _entitiesAreMoving = false;
            HeroGroup.GetDisplayedEntity().AnimationController.RestoreDefaultAnimation();
            HeroGroup.GetDisplayedEntity().MovementController.RotateTowardsCell(encounteredInterestPoint.cell);
            onInterestPointEncountered?.Invoke(encounteredInterestPoint.InterestPointConfiguration);
        }

        private void OnAllEntitiesMoved()
        {
            _entitiesAreMoving = false;
            try
            {
                _enemiesGroupSpawner.TrySpawnEntitiesGroup();
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
            if (_entitiesAreMoving) // To prevent the player from spaming movements
            {
                return;
            }

            Cell[] shorterPathToHoveredCell = CellsPathFinding.GetShorterPath(
                KingdomGrid,
                HeroGroup.cell,
                hoveredCell,
                checkLastCell: false
            );
            _currentHeroGroupMovePath = new(shorterPathToHoveredCell);
            HighlightShorterPathCells();
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
            foreach (Cell cell in _currentHeroGroupMovePath.path)
            {
                if (i < HeroGroup.movePoints)
                {
                    cell.HighlightController.Highlight(CellHighlightMaterial);
                }
                else
                {
                    cell.HighlightController.Highlight(CellInaccessibleHighlightMaterial);
                }
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (Cell cell in _currentHeroGroupMovePath.path)
            {
                cell.HighlightController.ResetToDefaultMaterial();
            }
        }
        #endregion

        #region Setup and tear down
        private void Awake()
        {
            if (_enemiesGroupSpawner == null)
            {
                _enemiesGroupSpawner = FindObjectOfType<EntitiesGroupsSpawner.EntitiesGroupsSpawner>();
            }
            if (_enemiesGroupSpawner == null)
            {
                Debug.LogError("No enemies groups spawner found. Enemies groups will not spawn.");
                return;
            }

            if (_kingdomLoader == null)
            {
                _kingdomLoader = FindObjectOfType<KingdomLoader>();
            }
            if (_kingdomLoader == null)
            {
                Debug.LogError("No kingdom loader found. Won't be able to correctly manage entities groups after fight.");
                return;
            }

            if (CellHighlightMaterial == null)
            {
                Debug.LogError("No highlight material provided. Can't highlight hovered cell.");
                return;
            }

            _enemiesGroupSpawner.onEntitiesGroupSpawned += OnEnemiesGroupSpawned;
            _kingdomLoader.onKingdomLoaded += OnKingdomLoaded;
        }

        private void Setup()
        {
            if (KingdomGrid == null)
            {
                KingdomGrid = FindObjectOfType<AHexGrid>();
            }
            if (KingdomGrid == null)
            {
                Debug.LogError("No HexGrid found in the scene. The Kingdom manager can't work.");
                gameObject.SetActive(false);
                return;
            }
            BindCellMouseEvents();

            HeroGroup = FindObjectsOfType<EntitiesGroup>().ToList().Find(entitiesGroup => entitiesGroup.name == "HeroGroup");
            EnemiesGroups = FindObjectsOfType<EntitiesGroup>().ToList().FindAll(entitiesGroup => entitiesGroup.name != "HeroGroup");

            _entitiesGroupsMovementController = new(KingdomGrid, HeroGroup);
            _entitiesGroupsMovementController.onAllEntitiesMoved += OnAllEntitiesMoved;
            _entitiesGroupsMovementController.onEnemiesGroupEncountered += OnEnemiesGroupEncounteredDuringMovement;
            _entitiesGroupsMovementController.onInterestPointEncountered += OnInterestPointEncountered;

            InterestPoints = FindObjectsOfType<InterestPoint>();

            BindOccupiersMouseEvents();
        }

        private void BindCellMouseEvents()
        {
            foreach (Cell cell in KingdomGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
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
