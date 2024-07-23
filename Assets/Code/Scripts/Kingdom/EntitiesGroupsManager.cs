using System;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Kingdom.EnemiesGroupsSpawner;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class EntitiesGroupsManager : MonoBehaviour
    {
        // Parameters are: Hero group, encountered enemies group, hero group initiating ?
        public Action<EntitiesGroup, EnemiesGroup, bool> onEnemiesGroupEncountered;

        [field: SerializeField] public Material CellHighlightMaterial { get; private set; }
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public HexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public List<EnemiesGroup> EnemiesGroups { get; private set; } = new();

        [SerializeField] private EnemiesGroupsSpawner.EnemiesGroupsSpawner _enemiesGroupSpawner;
        private EntitiesGroupsMovementController _entitiesGroupsMovementController;
        private MovePath _currentHeroGroupMovePath;
        private bool _entitiesAreMoving;

        // It's inside this function that the magic happens. It makes the hero group then the enemies move.
        private void OnCellClicked(Cell clickedCell)
        {
            if (_entitiesAreMoving || _currentHeroGroupMovePath.PathLength > HeroGroup.movePoints || !_currentHeroGroupMovePath.DoesNextCellExists() )
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

        private void OnEnemiesGroupEncounteredDuringMovement(EnemiesGroup encounteredEnemiesGroup, bool heroGroupHasInitiated)
        {
            onEnemiesGroupEncountered?.Invoke(HeroGroup, encounteredEnemiesGroup, heroGroupHasInitiated);
        }

        private void OnAllEntitiesMoved()
        {
            _entitiesAreMoving = false;
            try
            {
                _enemiesGroupSpawner.TrySpawnEnemiesGroup(GetOccupiedCells());   
            }
            catch (ImpossibleSpawnException)
            {
                Debug.Log("Could not spawn one more enemies group.");
            }
        }

        private void OnEnemiesGroupSpawned(EnemiesGroup spawnedEnemiesGroup)
        {
            EnemiesGroups.Add(spawnedEnemiesGroup);
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

        private void OnCellUnhovered(Cell hoveredCell)
        {
            ResetShorterPathCellsDefaultMaterial();
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
                _enemiesGroupSpawner = FindObjectOfType<EnemiesGroupsSpawner.EnemiesGroupsSpawner>();
            }
            if (_enemiesGroupSpawner == null)
            {
                Debug.LogError("No enemies groups spawner found. Enemies groups will not spawn.");
            }

            _entitiesGroupsMovementController = new();
            _entitiesGroupsMovementController.OnAllEntitiesMoved += OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered += OnEnemiesGroupEncounteredDuringMovement;
            _enemiesGroupSpawner.onEnemiesGroupSpawned += OnEnemiesGroupSpawned;
            BindCellMouseEvents();
        }

        private void OnDisable()
        {
            _entitiesGroupsMovementController.OnAllEntitiesMoved -= OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered -= OnEnemiesGroupEncounteredDuringMovement;
            _enemiesGroupSpawner.onEnemiesGroupSpawned -= OnEnemiesGroupSpawned;
            UnbindCellMouseEvents();
        }
        #endregion
    }
}
