using FrostfallSaga.Core;
using FrostfallSaga.Terrain;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    /// Represents a cell on an HexGrid.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [field: SerializeField, Header("Coordinates"), Tooltip("Contain coordinates")] public Vector2Int Coordinates { get; private set; }
        [field: SerializeField] public float WorldHeightPerUnit { get; private set; } = 0.8f;
        [field: SerializeField, Header("Cell characteristics"), Tooltip("Contain cell characteristics")] public TerrainTypeSO TerrainType { get; private set; }
        [field: SerializeField, Tooltip("Biome type")] public BiomeTypeSO BiomeType { get; private set; }
        [field: SerializeField] public ECellHeight Height { get; private set; }
        [field: SerializeField] public bool IsAccessible { get; private set; }
        [field: SerializeField, Header("Controllers"), Tooltip("Contain all controllers")] public MaterialHighlightable HighlightController { get; private set; }
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
            float hexSize,
            TerrainTypeSO terrainType,
            BiomeTypeSO biomeType
        )
        {

            Coordinates = coordinates;
            Height = cellHeight;
            TerrainType = terrainType;
            BiomeType = biomeType;
            IsAccessible = terrainType.IsAccessible;
            SetPositionForCellHeight(Height);
            SetCellMouseEventsControllerFromGameObjectTree();
            SetTerrainVisual();

            HighlightController = GetComponentInChildren<MaterialHighlightable>();
            if (HighlightController != null)
            {
                HighlightController.transform.localScale = Vector3.one * hexSize / 2.68f;
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
            Vector3 center = HexMetrics.Center(grid.HexSize, Coordinates.x, Coordinates.y, grid.HexOrientation);
            center.y = GetYPosition();
            return center;
        }

        public float GetYPosition()
        {
            return WorldHeightPerUnit + ((int)Height + 1);
        }

        private void SetPositionForCellHeight(ECellHeight cellHeight)
        {
            transform.position = new Vector3(transform.position.x, (float)cellHeight, transform.position.z);
        }

        private void SetCellVisualFromGameObjectTree()
        {
            HighlightController = GetComponentInChildren<MaterialHighlightable>();
            if (HighlightController == null)
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

        private void SetTerrainVisual()
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer != null && TerrainType != null && TerrainType.CellMaterial != null)
            {
                renderer.material = TerrainType.CellMaterial;
            }
            else
            {
                Debug.LogError("Cell " + name + " doesn't have a renderer or a valid material.");
            }
        }
    }
}
