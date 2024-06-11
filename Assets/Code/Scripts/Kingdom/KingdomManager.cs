using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using System.Collections;

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
        [field: SerializeField] public EntityGroup HeroGroup { get; private set; }
        [field: SerializeField] public EnemiesGroup[] EnemiesGroups { get; private set; } = { };
        [field: SerializeField] public bool GenerateRandomEnemiesGroupsAtStart { get; private set; } = true;

        private Cell[] _currentShorterPath;
        private bool _heroGroupIsMoving;

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

        private void OnCellClicked(Cell clickedCell)
        {
            if (_currentShorterPath.Length > HeroGroup.MovePoints)
            {
                return;
            }

            ResetShorterPathCellsDefaultMaterial();
            StartCoroutine(MoveHeroGroup());
        }

        private IEnumerator MoveHeroGroup()
        {
            _heroGroupIsMoving = true;
            foreach (Cell cell in _currentShorterPath)
            {
                HeroGroup.MoveToCell(cell);
                yield return new WaitForSeconds(1); // TODO: Will change with real animation
            }
            _heroGroupIsMoving = false;
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            if (_heroGroupIsMoving)
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
