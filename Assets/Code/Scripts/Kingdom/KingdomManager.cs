using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Responsible for managing entities movements and encounters.
    /// </summary>
    public class KingdomManager : MonoBehaviour
    {
        private HexGrid _kingdomGrid;
        [field: SerializeField] Material _cellHighlightMaterial;

        private void OnEnable()
        {
            _kingdomGrid = FindObjectOfType<HexGrid>();
            if (_kingdomGrid == null)
            {
                Debug.LogError("No HexGrid found in the scene. The Kingdom manager can't work.");
                gameObject.SetActive(false);
                return;
            }

            foreach (Cell cell in _kingdomGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
            }
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            if (_cellHighlightMaterial == null)
            {
                Debug.LogError("No highlight material provided. Can't highlight hovered cell.");
                return;
            }

            hoveredCell.CellVisual.Highlight(_cellHighlightMaterial);
        }

        private void OnCellUnhovered(Cell hoveredCell)
        {
            hoveredCell.CellVisual.ResetMaterial();
        }

        private void OnCellClicked(Cell clickedCell)
        {
            Debug.Log("Clicked on cell with coordinates: " + clickedCell.Coordinates);
        }

        private void OnDisable()
        {
            if (_kingdomGrid == null)
            {
                Debug.Log("Grid already disabled or destroyed.");
            }
            else
            {
                foreach (Cell cell in _kingdomGrid.GetCells())
                {
                    cell.CellMouseEventsController.OnElementHover -= OnCellHovered;
                    cell.CellMouseEventsController.OnElementUnhover -= OnCellUnhovered;
                    cell.CellMouseEventsController.OnLeftMouseDown -= OnCellClicked;
                }
            }
        }
    }
}
