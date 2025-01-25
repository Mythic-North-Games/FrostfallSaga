using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.CityBuildings;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.EntitiesGroupsSpawner;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class EntitiesGroupsManager : MonoBehaviour
    {
        // Parameters are: Hero group, encountered enemies group, hero group initiating ?
        public Action<EntitiesGroup, EntitiesGroup, bool> onEnemiesGroupEncountered;
        public Action<CityBuilding> onCityEncountered;

        [field: SerializeField] public Material CellHighlightMaterial { get; private set; }
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public HexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public List<EntitiesGroup> EnemiesGroups { get; private set; } = new();
        [field: SerializeField] public CityBuilding[] CityBuildings { get; private set; }

        [SerializeField] private EntitiesGroupsSpawner.EntitiesGroupsSpawner _enemiesGroupSpawner;
        [SerializeField] private KingdomLoader _kingdomLoader;
        private EntitiesGroupsMovementController _entitiesGroupsMovementController;
        private MovePath _currentHeroGroupMovePath;
        private bool _entitiesAreMoving;

        private void OnKingdomLoaded()
        {
            Setup();
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

        private void OnEntitiesGroupClicked(EntitiesGroup clickedEntitiesGroup)
        {
            OnCellClicked(clickedEntitiesGroup.cell);
        }

        private void OnCityClicked(CityBuilding clickedCity)
        {
            OnCellClicked(clickedCity.cell);
        }

        private void OnEnemiesGroupEncounteredDuringMovement(EntitiesGroup encounteredEnemiesGroup, bool heroGroupHasInitiated)
        {
            onEnemiesGroupEncountered?.Invoke(HeroGroup, encounteredEnemiesGroup, heroGroupHasInitiated);
        }

        private void OnCityEncountered(CityBuilding encounteredCity)
        {
            _entitiesAreMoving = false;
            HeroGroup.GetDisplayedEntity().AnimationController.RestoreDefaultAnimation();
            HeroGroup.GetDisplayedEntity().MovementController.RotateTowardsCell(encounteredCity.cell);
            onCityEncountered?.Invoke(encounteredCity);
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
            spawnedEnemiesGroup.onEntityGroupHovered += OnEntitiesGroupHovered;
            spawnedEnemiesGroup.onEntityGroupUnhovered += OnEntitiesGroupUnhovered;
            spawnedEnemiesGroup.onEntityGroupClicked += OnEntitiesGroupClicked;
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

        private void OnEntitiesGroupHovered(EntitiesGroup hoveredEntitiesGroup)
        {
            OnCellHovered(hoveredEntitiesGroup.cell);
        }

        private void OnCityHovered(CityBuilding hoveredCity)
        {
            OnCellHovered(hoveredCity.cell);
        }

        private void OnCellUnhovered(Cell hoveredCell)
        {
            ResetShorterPathCellsDefaultMaterial();
        }

        private void OnEntitiesGroupUnhovered(EntitiesGroup unhoveredEntitiesGroup)
        {
            OnCellUnhovered(unhoveredEntitiesGroup.cell);
        }

        private void OnCityUnhovered(CityBuilding unhoveredCity)
        {
            OnCellUnhovered(unhoveredCity.cell);
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

        #region Cell mouse events binding and unbinding
        private void BindCellMouseEvents()
        {
            foreach (Cell cell in KingdomGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
            }
        }
        #endregion

        #region Entities groups mouse events binding and unbinding
        private void BindEntitiesGroupsMouseEvents()
        {
            HeroGroup.onEntityGroupHovered += OnEntitiesGroupHovered;
            HeroGroup.onEntityGroupUnhovered += OnEntitiesGroupUnhovered;
            HeroGroup.onEntityGroupClicked += OnEntitiesGroupClicked;
            EnemiesGroups.ForEach(enemiesGroup =>
            {
                enemiesGroup.onEntityGroupHovered += OnEntitiesGroupHovered;
                enemiesGroup.onEntityGroupUnhovered += OnEntitiesGroupUnhovered;
                enemiesGroup.onEntityGroupClicked += OnEntitiesGroupClicked;
            });
        }
        #endregion

        #region Cities mouse events binding and unbinding
        private void BindCitiesMouseEvents()
        {
            foreach (CityBuilding city in CityBuildings)
            {
                city.MouseEventsController.OnElementHover += OnCityHovered;
                city.MouseEventsController.OnElementUnhover += OnCityUnhovered;
                city.MouseEventsController.OnLeftMouseUp += OnCityClicked;
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
                KingdomGrid = FindObjectOfType<HexGrid>();
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
            BindEntitiesGroupsMouseEvents();

            _entitiesGroupsMovementController = new(KingdomGrid, HeroGroup);
            _entitiesGroupsMovementController.OnAllEntitiesMoved += OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered += OnEnemiesGroupEncounteredDuringMovement;
            _entitiesGroupsMovementController.OnCityEncountered += OnCityEncountered;

            CityBuildings = FindObjectsOfType<CityBuilding>();
            BindCitiesMouseEvents();
        }
        #endregion
    }
}
