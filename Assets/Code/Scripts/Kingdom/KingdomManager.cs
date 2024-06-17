using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class KingdomManager : MonoBehaviour
    {
        [field: SerializeField] public Material CellHighlightMaterial { get; private set; }
        [field: SerializeField] public Material CellInaccessibleHighlightMaterial { get; private set; }
        [field: SerializeField] public HexGrid KingdomGrid { get; private set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; private set; }
        [field: SerializeField] public EnemiesGroup[] EnemiesGroups { get; private set; } = { };        
        public bool ShowEnemiesGroupsMovePath = false;

        private Cell[] _currentShorterPath;
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

            BindCellMouseEvents();
        }

        // It's inside this function that the magic happens. It makes the hero group then the enemies move.
        private void OnCellClicked(Cell clickedCell)
        {
            if (_entitiesAreMoving || _currentShorterPath.Length > HeroGroup.MovePoints)
            {
                return;
            }

            _entitiesAreMoving = true;
            ResetShorterPathCellsDefaultMaterial();
            StartCoroutine(MakeHeroGroupThenEnemiesGroupsMove());
            _entitiesAreMoving = false;
        }

        #region Entities movements handling
        private IEnumerator MakeHeroGroupThenEnemiesGroupsMove()
        {
            foreach (Cell cell in _currentShorterPath)
            {
                HeroGroup.MoveToCell(cell);
                yield return new WaitForSeconds(1); // TODO: Will change with real animation
            }
            MakeAllEnemiesGroupsMoveSimultaneously();
        }

        private void MakeAllEnemiesGroupsMoveSimultaneously()
        {
            Dictionary<EntitiesGroup, Cell[]> movePathPerEnemiesGroup = EntitiesGroupsMovementController.GenerateRandomMovePathPerEntitiesGroup(KingdomGrid, EnemiesGroups);
            foreach (KeyValuePair<EntitiesGroup, Cell[]> item in movePathPerEnemiesGroup)
            {
                StartCoroutine(MakeEnemiesGroupMove((EnemiesGroup)item.Key, item.Value));
            }
        }

        private IEnumerator MakeEnemiesGroupMove(EnemiesGroup enemiesGroup, Cell[] movePath)
        {
            if (ShowEnemiesGroupsMovePath)
            {
                foreach (Cell cellOfPath in movePath)
                {
                    cellOfPath.CellVisual.Highlight(CellHighlightMaterial);
                }
            }

            foreach (Cell cellOfPath in movePath)
            {
                enemiesGroup.MoveToCell(cellOfPath);
                yield return new WaitForSeconds(1); // TODO: Will change with real animation
                if (ShowEnemiesGroupsMovePath)
                {
                    cellOfPath.CellVisual.ResetMaterial();
                }
            }
        }
        #endregion

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

            _currentShorterPath = CellsPathFinding.GetShorterPath(KingdomGrid, HeroGroup.Cell, hoveredCell);
            HighlightShorterPathCells();
        }

        private void OnCellUnhovered(Cell hoveredCell)
        {
            foreach (Cell cell in _currentShorterPath)
            {
                cell.CellVisual.ResetMaterial();
            }
        }

        private void HighlightShorterPathCells()
        {
            int i = 0;
            foreach (Cell cell in _currentShorterPath)
            {
                if (i < HeroGroup.MovePoints)
                {
                    cell.CellVisual.Highlight(CellHighlightMaterial);
                }
                else
                {
                    cell.CellVisual.Highlight(CellInaccessibleHighlightMaterial);
                }
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (Cell cell in _currentShorterPath)
            {
                cell.CellVisual.ResetMaterial();
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

        private void OnDisable()
        {
            UnbindCellMouseEvents();
        }
        #endregion
    }
}
