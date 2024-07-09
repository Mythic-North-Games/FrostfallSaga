using System;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class EntitiesGroupsManager : MonoBehaviour
    {
        [field: SerializeField] public Material CellHighlightMaterial { get; private set; }
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public HexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public EnemiesGroup[] EnemiesGroups { get; private set; } = { };

        // Parameters are: Hero group, encountered enemies group, hero group initiating ?
        public Action<EntitiesGroup, EnemiesGroup, bool> OnEnemiesGroupEncountered;

        private EntitiesGroupsMovementController _entitiesGroupsMovementController;
        private MovePath _currentHeroGroupMovePath;
        private bool _entitiesAreMoving;

        private void Awake()
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

            _entitiesGroupsMovementController = new();
            _entitiesGroupsMovementController.OnAllEntitiesMoved += OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered += OnEnemiesGroupEncounteredDuringMovement;
            BindCellMouseEvents();
        }

        // It's inside this function that the magic happens. It makes the hero group then the enemies move.
        private void OnCellClicked(Cell clickedCell)
        {
            if (_entitiesAreMoving || _currentHeroGroupMovePath.PathLength > HeroGroup.MovePoints)
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
                EnemiesGroups
            );
        }

        private void OnEnemiesGroupEncounteredDuringMovement(EnemiesGroup encounteredEnemiesGroup, bool heroGroupHasInitiated)
        {
            OnEnemiesGroupEncountered?.Invoke(HeroGroup, encounteredEnemiesGroup, heroGroupHasInitiated);
        }

        private void OnAllEntitiesMoved()
        {
            _entitiesAreMoving = false;
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

            _currentHeroGroupMovePath = new(CellsPathFinding.GetShorterPath(KingdomGrid, HeroGroup.Cell, hoveredCell));
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
                if (i < HeroGroup.MovePoints)
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

        private void OnDisable()
        {
            UnbindCellMouseEvents();
            _entitiesGroupsMovementController.OnAllEntitiesMoved -= OnAllEntitiesMoved;
            _entitiesGroupsMovementController.OnEnemiesGroupEncountered -= OnEnemiesGroupEncounteredDuringMovement;
        }
    }
}
