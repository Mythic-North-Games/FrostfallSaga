using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    /// Represents a cell on an HexGrid.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        private float _size;
        private ECellOrientation _orientation;
        [field: SerializeField] public Vector2Int Coordinates { get; private set; }
        [field: SerializeField] public ECellHeight Height { get; private set; }
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
        /// <param name="hexSize">The size of a cell inside the grid.</param>
        public void Setup(
            Vector2Int coordinates,
            ECellHeight cellHeight,
            bool isAccessible,
            float hexSize
        )
        {

            Coordinates = coordinates;
            Height = cellHeight;
            IsAccessible = isAccessible;
            SetPositionForCellHeight(Height);
            SetCellMouseEventsControllerFromGameObjectTree();

            CellVisual = GetComponentInChildren<CellVisual>();
            if (CellVisual != null)
            {
                CellVisual.transform.localScale = Vector3.one * hexSize / 2.68f;
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
            Height = newCellHeight;
            SetPositionForCellHeight(Height);
        }

        public Vector3 GetCenter()
        {
            HexGrid grid = GetComponentInParent<HexGrid>();
            return HexMetrics.Center(grid.HexSize, Coordinates.x, Coordinates.y, grid.HexOrientation);
        }

        private void SetPositionForCellHeight(ECellHeight cellHeight)
        {
            transform.position = new Vector3(transform.position.x, (float)cellHeight, transform.position.z);
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
