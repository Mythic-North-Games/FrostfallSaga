using FrostfallSaga.Core;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    /// Represents a cell on an HexGrid.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [field: SerializeField, Header("Coordinates"), Tooltip("Contain coordinates")] public Vector2Int Coordinates { get; private set; }
        [property: SerializeField]
        public Vector2Int AxialCoordinates
        {
            get { return HexMetrics.OffsetToAxial(Coordinates); }
        }
        [field: SerializeField] public float WorldHeightPerUnit { get; private set; } = 0.8f;
        [field: SerializeField, Header("Cell characteristics"), Tooltip("Contain cell characteristics")] public TerrainTypeSO TerrainType { get; private set; }
        [field: SerializeField, Tooltip("Biome type")] public BiomeTypeSO BiomeType { get; private set; }
        [field: SerializeField] public ECellHeight Height { get; private set; }

        [field: SerializeField, Header("Controllers"), Tooltip("Contain all controllers")] public MaterialHighlightable HighlightController { get; private set; }
        [field: SerializeField] public CellMouseEventsController CellMouseEventsController { get; private set; }
        private HexGrid ParentGrid;

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
            ParentGrid = GetComponentInParent<HexGrid>();
            SetTerrain(terrainType);
            SetPositionForCellHeight(Height);
            SetCellMouseEventsControllerFromGameObjectTree();
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
        /// Returns if the cell is accessible, regardless of the possible cell occupants.
        /// </summary>
        /// <returns>True if the terrain is accessible and if there are no obstacles, false otherwise</returns>
        public virtual bool IsTerrainAccessible()
        {
            return TerrainType.IsAccessible;
        }

        /// <summary>
        /// Returns if the cell is free to be occupied.
        /// </summary>
        /// <returns>True if the cell is accessible and contains no occupants.</returns>
        public virtual bool IsFree()
        {
            return TerrainType.IsAccessible;
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

        public void SetTerrain(TerrainTypeSO terrainType)
        {
            TerrainType = terrainType;
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer != null && TerrainType != null && TerrainType.CellMaterial != null)
            {
                if (TerrainType.VisualsTerrain.Length != 0)
                {
                    GameObject visualTerrain = Randomizer.GetRandomElementFromArray(TerrainType.VisualsTerrain);
                    GameObject newVisualTerrain = Instantiate<GameObject>(visualTerrain, transform.position, Randomizer.GetRandomRotationY(transform.rotation), transform);
                    newVisualTerrain.name = "Visual" + name;
                }
            }
            else
            {
                Debug.LogError("Cell " + name + " doesn't have a renderer or a valid material.");
            }
        }

        public Vector3 GetCenter()
        {
            Vector3 center = HexMetrics.Center(ParentGrid.HexSize, Coordinates.x, Coordinates.y, ParentGrid.HexOrientation);
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

        public static Vector2Int GetHexDirection(Cell initiatorCell, Cell targetCell)
        {
            Vector2Int initiatorAxial = initiatorCell.AxialCoordinates;
            Vector2Int targetAxial = targetCell.AxialCoordinates;
            return targetAxial - initiatorAxial;
        }
    }
}
