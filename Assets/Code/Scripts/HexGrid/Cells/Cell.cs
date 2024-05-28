using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    /// Represents a cell on an HexGrid.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [field: SerializeField] public Vector2Int Coordinates { get; private set; }
        [field: SerializeField] public ECellHeight CellHeight { get; private set; }
        [field: SerializeField] public bool IsAccessible { get; private set; }
        [field: SerializeField] public CellVisual CellVisual { get; private set; }
        [field: SerializeField] public CellMouseEventsController CellMouseEventsController { get; private set; }

        private void Awake()
        {
            SetCellVisualFromGameObjectTree();
            SetCellMouseEventsControllerFromGameObjectTree();
        }

        /// <summary>
        /// Setup the cell with the given property. The cell should already be attached to a spawned GameObject.
        /// ⚠️ This method should only be called when you want to instanciate a cell in the first place.
        /// </summary>
        /// <param name="coordinates">The cell coordinates on the grid.</param>
        /// <param name="cellHeight">The cell height.</param>
        /// <param name="isAccessible">The accessibility of the cell.</param>
        /// <param name="hexGridSize">The size of the grid to setup the cell into (used for cell visual local scale).</param>
        public void Setup(Vector2Int coordinates, ECellHeight cellHeight, bool isAccessible, float hexGridSize)
        {
            Coordinates = coordinates;
            CellHeight = cellHeight;
            IsAccessible = isAccessible;
            SetPositionForCellHeight(CellHeight);
            SetCellMouseEventsControllerFromGameObjectTree();

            CellVisual = GetComponentInChildren<CellVisual>();
            if (CellVisual != null)
            {
                CellVisual.transform.localScale = Vector3.one * hexGridSize / 2.68f;
            }
            else
            {
                Debug.LogError("Cell " + name + " has no visual to be set up. Please add a cell visual as a child.");
            }
        }

        /// <summary>
        /// Updates the cell height and y position in the world.
        /// </summary>
        /// <param name="newCellHeight">The new cell height.</param>
        public void UpdateHeight(ECellHeight newCellHeight)
        {
            CellHeight = newCellHeight;
            SetPositionForCellHeight(CellHeight);
        }

        /// <summary>
        /// Compute and returns the current cell neighbors in the given grid.
        /// </summary>
        /// <param name="hexGrid">The grid the current cell is considered inside.</param>
        /// <param name="onlyAccessibleNeighbors">If only the accessible cells should be returned.</param>
        /// <returns>The current cell neighbors in the given grid.</returns>
        public Cell[] GetNeighbors(HexGrid hexGrid, bool onlyAccessibleNeighbors = true)
        {
            List<Cell> neighbors = new();
            Vector2Int[] directionsToCheck = new Vector2Int[]
            {
                new(1, 0), new(-1, 0),
                new(0, 1), new(0, -1),
                new(1, -1), new(-1, 1)
            };

            foreach (Vector2Int direction in directionsToCheck)
            {
                Vector2Int neighborCoord = Coordinates + direction;
                Dictionary<Vector2Int, Cell> cellsByCoordinates = hexGrid.CellsByCoordinates;
                if (
                    cellsByCoordinates.ContainsKey(neighborCoord) && (
                        !onlyAccessibleNeighbors || onlyAccessibleNeighbors && cellsByCoordinates[neighborCoord].IsAccessible
                    )
                )
                {
                    neighbors.Add(cellsByCoordinates[neighborCoord]);
                }
            }

            return neighbors.ToArray();
        }

        private void SetPositionForCellHeight(ECellHeight cellHeight)
        {
            Vector3 updatedPosition = transform.position;
            updatedPosition.y = cellHeight switch
            {
                ECellHeight.LOW => -1f,
                ECellHeight.MEDIUM => 0f,
                ECellHeight.HIGH => 1f,
                _ => -1f,
            };
            transform.position = updatedPosition;
        }

        private void SetCellVisualFromGameObjectTree()
        {
            CellVisual = GetComponentInChildren<CellVisual>();
            if (CellVisual == null)
            {
                Debug.LogError("Cell " + name + " doesn't have a cell visual as child.");
            }
        }

        private void SetCellMouseEventsControllerFromGameObjectTree()
        {
            CellMouseEventsController = GetComponentInChildren<CellMouseEventsController>();
            if (CellMouseEventsController == null)
            {
                Debug.LogError("Cell " + name + " doesn't have a cell mouse controller as child.");
            }
        }
    }
}
