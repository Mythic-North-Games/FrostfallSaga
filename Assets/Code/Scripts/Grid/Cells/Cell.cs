using System.Collections;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    ///     Represents a cell on an HexGrid.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [field: SerializeField]
        [field: Header("Coordinates")]
        [field: Tooltip("Contain coordinates")]
        public Vector2Int Coordinates { get; set; }

        [field: SerializeField] public float WorldHeightPerUnit { get; private set; } = 0.8f;

        [field: SerializeField]
        [field: Header("Cell characteristics")]
        [field: Tooltip("Contain cell characteristics")]
        public TerrainTypeSO TerrainType { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Biome type")]
        public BiomeTypeSO BiomeType { get; private set; }

        [field: SerializeField] public ECellHeight Height { get; private set; }
        [field: SerializeField] public float UpdateHeightDuration { get; private set; }

        [field: SerializeField]
        [field: Header("Controllers")]
        [field: Tooltip("Contain all controllers")]
        public MaterialHighlightable HighlightController { get; private set; }

        [field: SerializeField] public CellMouseEventsController CellMouseEventsController { get; private set; }
        private AHexGrid _parentGrid;

        public Vector2Int AxialCoordinates => HexMetrics.OffsetToAxial(Coordinates);

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            SetParentGridFromGameObjectTree();
            SetCellVisualFromGameObjectTree();
            SetCellMouseEventsControllerFromGameObjectTree();
        }

        /// <summary>
        ///     Setup the cell with the given property. The cell should already be attached to a spawned GameObject.
        ///     ⚠️ This method should only be called when you want to instanciate a cell in the first place.
        /// </summary>
        /// <param name="coordinates">The cell coordinates on the grid.</param>
        /// <param name="cellHeight">The cell height.</param>
        /// <param name="hexSize">The size of a cell inside the grid.</param>
        /// <param name="terrainType">The type of terrain</param>
        /// <param name="biomeType">The type of biome</param>
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
            SetTerrain(terrainType);
            SetPositionForCellHeight(Height, UpdateHeightDuration);
            SetCellMouseEventsControllerFromGameObjectTree();

            HighlightController = GetComponentInChildren<MaterialHighlightable>();
            if (HighlightController)
                HighlightController.transform.localScale = Vector3.one * hexSize / 2.68f;
            else
                Debug.LogError("Cell " + name + " has no visual to be set up. Please add a cell visual as a child.");
        }

        /// <summary>
        ///     Returns if the cell is accessible, regardless of the possible cell occupants.
        /// </summary>
        /// <returns>True if the terrain is accessible and if there are no obstacles, false otherwise</returns>
        public virtual bool IsTerrainAccessible()
        {
            return TerrainType.IsAccessible;
        }

        /// <summary>
        ///     Returns if the cell is free to be occupied.
        /// </summary>
        /// <returns>True if the cell is accessible and contains no occupants.</returns>
        public virtual bool IsFree()
        {
            return TerrainType.IsAccessible;
        }

        /// <summary>
        ///     Updates the cell height and y position in the world.
        /// </summary>
        /// <param name="newCellHeight">The new cell height.</param>
        public void UpdateHeight(ECellHeight newCellHeight, float cellAlterationDuration)
        {
            Height = newCellHeight;
            SetPositionForCellHeight(Height, cellAlterationDuration);
        }

        public void SetTerrain(TerrainTypeSO terrainType)
        {
            TerrainType = terrainType;
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer && TerrainType && TerrainType.CellMaterial)
            {
                if (TerrainType.VisualsTerrain != null && TerrainType.VisualsTerrain.Length != 0)
                {
                    GameObject visualTerrain = Randomizer.GetRandomElementFromArray(TerrainType.VisualsTerrain);
                    GameObject newVisualTerrain = Instantiate(visualTerrain, transform.position,
                        Randomizer.GetRandomRotationY(transform.rotation), transform);
                    newVisualTerrain.name = "Visual" + name;
                    LayerUtils.SetLayerRecursively(newVisualTerrain, 2);
                }
            }
            else
            {
                Debug.LogError("Cell " + name + " doesn't have a renderer or a valid material.");
            }
        }

        public Vector3 GetCenter()
        {
            Vector3 center = HexMetrics.Center(_parentGrid.HexSize, Coordinates.x, Coordinates.y);
            center.y = GetYPosition();
            return center;
        }

        public float GetYPosition()
        {
            return WorldHeightPerUnit + ((int)Height + 1);
        }

        private void SetPositionForCellHeight(ECellHeight cellHeight, float duration)
        {
            if (duration == 0)
                transform.position = new Vector3(transform.position.x, (float)cellHeight, transform.position.z);
            else
                StartCoroutine(SmoothMoveToHeight(cellHeight, duration));
        }

        private IEnumerator SmoothMoveToHeight(ECellHeight targetHeight, float duration)
        {
            var startHeight = transform.position.y;
            var targetY = (float)targetHeight;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var newY = Mathf.Lerp(startHeight, targetY, elapsedTime / duration);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                yield return null;
            }

            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }

        private void SetParentGridFromGameObjectTree()
        {
            _parentGrid ??= GetComponentInParent<AHexGrid>();
            if (_parentGrid == null) Debug.LogError("Cell " + name + " doesn't have parent Grid.");
        }

        private void SetCellVisualFromGameObjectTree()
        {
            HighlightController ??= GetComponentInChildren<MaterialHighlightable>();
            if (HighlightController == null) Debug.LogError("Cell " + name + " doesn't have a cell visual as child.");
        }

        private void SetCellMouseEventsControllerFromGameObjectTree()
        {
            CellMouseEventsController ??= GetComponentInChildren<CellMouseEventsController>();
            if (!CellMouseEventsController)
                Debug.LogError("Cell " + name + " doesn't have a cell mouse controller as child.");
        }

        public static Vector2Int GetHexDirection(Cell initiatorCell, Cell targetCell)
        {
            Vector2Int initiatorAxial = initiatorCell.AxialCoordinates;
            Vector2Int targetAxial = targetCell.AxialCoordinates;
            return targetAxial - initiatorAxial;
        }

        public void SetParentGridForTests(AHexGrid grid)
        {
            _parentGrid = grid;
        }

        public override string ToString()
        {
            return "Cell: \n" +
                   $"- Coordinates: {Coordinates}\n" +
                   $"- AxialCoordinates: {AxialCoordinates}\n" +
                   $"- WorldHeightPerUnit: {WorldHeightPerUnit}\n" +
                   $"- TerrainType: {TerrainType?.name ?? "None"}\n" +
                   $"- BiomeType: {BiomeType?.name ?? "None"}\n" +
                   $"- Height: {Height}\n" +
                   $"- UpdateHeightDuration: {UpdateHeightDuration}\n" +
                   $"- HighlightController: {(HighlightController != null ? "Set" : "Not Set")}\n" +
                   $"- CellMouseEventsController: {(CellMouseEventsController != null ? "Set" : "Not Set")}";
        }
    }
}