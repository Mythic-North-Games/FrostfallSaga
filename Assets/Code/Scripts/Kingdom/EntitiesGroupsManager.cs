using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.EntitiesGroupsSpawner;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class EntitiesGroupsManager : MonoBehaviour
    {
        // Parameters are: Hero group, encountered enemies group, hero group initiating ?
        public Action<EntitiesGroup, EntitiesGroup, bool> onEnemiesGroupEncountered;

        [field: SerializeField] public Material CellHighlightMaterial { get; private set; }
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public HexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public List<EntitiesGroup> EnemiesGroups { get; private set; } = new();

        [SerializeField] private EntitiesGroupsSpawner.EntitiesGroupsSpawner _enemiesGroupSpawner;
        [SerializeField] private KingdomLoader _kingdomLoader;
        private EntitiesGroupsMovementController _entitiesGroupsMovementController;
        private MovePath _currentHeroGroupMovePath;
        private bool _entitiesAreMoving;

        private void OnKingdomLoaded()
        {
            HeroGroup = FindObjectsOfType<EntitiesGroup>().ToList().Find(entitiesGroup => entitiesGroup.name == "HeroGroup");
            EnemiesGroups = FindObjectsOfType<EntitiesGroup>().ToList().FindAll(entitiesGroup => entitiesGroup.name != "HeroGroup");
            BindEntitiesGroupsMouseEvents();
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
                KingdomGrid,
                HeroGroup,
                _currentHeroGroupMovePath,
                EnemiesGroups.ToArray()
            );

        }

        private void OnEntitiesGroupClicked(EntitiesGroup clickedEntitiesGroup)
        {
            OnCellClicked(clickedEntitiesGroup.cell);
        }

        private void OnEnemiesGroupEncounteredDuringMovement(EntitiesGroup encounteredEnemiesGroup, bool heroGroupHasInitiated)
        {
            onEnemiesGroupEncountered?.Invoke(HeroGroup, encounteredEnemiesGroup, heroGroupHasInitiated);
        }

        private void OnAllEntitiesMoved()
        {
            _entitiesAreMoving = false;
            try
            {
                _enemiesGroupSpawner.TrySpawnEntitiesGroup(GetOccupiedCells());
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

        private Cell[] GetOccupiedCells()
        {
            List<Cell> occupiedCells = new()
            {
                HeroGroup.cell
            };
            EnemiesGroups.ForEach(group => occupiedCells.Add(group.cell));
            return occupiedCells.ToArray();
        }

        #region Cells hovering and highlighting
        private void OnCellHovered(Cell hoveredCell)
        {
            if (_entitiesAreMoving) // To prevent the player from spaming movements
            {
                return;
            }

            if (CellHighlightMaterial == null)
            {
                Debug.LogError("No highlight material provided. Can't highlight hovered cell.");
                return;
            }

            _currentHeroGroupMovePath = new(CellsPathFinding.GetShorterPath(KingdomGrid, HeroGroup.cell, hoveredCell));
            HighlightShorterPathCells();
        }

        private void OnEntitiesGroupHovered(EntitiesGroup hoveredEntitiesGroup)
        {
            OnCellHovered(hoveredEntitiesGroup.cell);
        }

        private void OnCellUnhovered(Cell hoveredCell)
        {
            ResetShorterPathCellsDefaultMaterial();
        }

        private void OnEntitiesGroupUnhovered(EntitiesGroup unhoveredEntitiesGroup)
        {
            OnCellUnhovered(unhoveredEntitiesGroup.cell);
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

        private void UnbindCellMouseEvents()
        {
            if (KingdomGrid == null)
            {
                Debug.Log("Grid already disabled or destroyed.");
            }
            else
            {
                foreach (Cell cell in KingdomGrid.GetCells())
                {
                    cell.CellMouseEventsController.OnElementHover -= OnCellHovered;
                    cell.CellMouseEventsController.OnElementUnhover -= OnCellUnhovered;
                    cell.CellMouseEventsController.OnLeftMouseDown -= OnCellClicked;
                }
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

        private void UnbindEntitiesGroupsMouseEvents()
        {
            HeroGroup.onEntityGroupHovered -= OnEntitiesGroupHovered;
            HeroGroup.onEntityGroupUnhovered -= OnEntitiesGroupUnhovered;
            HeroGroup.onEntityGroupClicked -= OnEntitiesGroupClicked;
            EnemiesGroups.ForEach(enemiesGroup =>
            {
                enemiesGroup.onEntityGroupHovered -= OnEntitiesGroupHovered;
                enemiesGroup.onEntityGroupUnhovered -= OnEntitiesGroupUnhovered;
                enemiesGroup.onEntityGroupClicked -= OnEntitiesGroupClicked;
            });
        }
        #endregion

        #region Setup and tear down
        private void OnEnable()
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

            _enemiesGroupSpawner.onEntitiesGroupSpawned += OnEnemiesGroupSpawned;
            _kingdomLoader.onKingdomLoaded += OnKingdomLoaded;
            _entitiesGroupsMovementController = new();
            _entitiesGroupsMovementController.OnAllEntitiesMoved += OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered += OnEnemiesGroupEncounteredDuringMovement;
            BindCellMouseEvents();

            if (HeroGroup != null)
            {
                BindEntitiesGroupsMouseEvents();
            }
        }

        private void OnDisable()
        {
            _entitiesGroupsMovementController.OnAllEntitiesMoved -= OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered -= OnEnemiesGroupEncounteredDuringMovement;
            _enemiesGroupSpawner.onEntitiesGroupSpawned -= OnEnemiesGroupSpawned;
            UnbindCellMouseEvents();

            if (HeroGroup != null)
            {
                UnbindEntitiesGroupsMouseEvents();
            }
        }
        #endregion
    }
}
