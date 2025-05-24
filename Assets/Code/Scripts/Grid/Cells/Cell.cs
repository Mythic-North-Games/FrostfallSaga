using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    ///     Represents a cell on an HexGrid.
    /// </summary>
    public abstract class Cell : MonoBehaviour
    {
        [field: SerializeField]
        [field: Header("Coordinates")]
        [field: Tooltip("Contain coordinates")]
        public Vector2Int Coordinates { get; set; }

        [field: SerializeField] 
        public float WorldHeightPerUnit { get; private set; } = 0.8f;

        [field: SerializeField]
        [field: Header("Cell characteristics")]
        [field: Tooltip("Contain cell characteristics")]
        public TerrainTypeSO TerrainType { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Biome type")]
        public BiomeTypeSO BiomeType { get; private set; }

        [field: SerializeField] 
        public ECellHeight Height { get; private set; }
        
        [field: SerializeField] 
        public float UpdateHeightDuration { get; private set; }

        [field: SerializeField]
        [field: Header("Controllers")]
        [field: Tooltip("Contain all controllers")]
        public MaterialHighlightable HighlightController { get; private set; }

        private AHexGrid _parentGrid;
        private CellVisualController _cellVisualController;
        private CellTerrainAccessibilityController _cellTerrainAccessibilityController;
        private CellPositionController _cellPositionController;

        public bool IsAccessible => _cellTerrainAccessibilityController?.IsAccessible ?? false;
        public Vector2Int AxialCoordinates => HexMetrics.OffsetToAxial(Coordinates);

        public bool HasVisual => _cellVisualController.VisualTransform != null;
        public string VisualPrefabPath => _cellVisualController.PrefabPath;
        public Transform VisualTransform => _cellVisualController.VisualTransform;
        

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _parentGrid ??= GetComponentInParent<AHexGrid>();
            if (!_parentGrid) Debug.LogError("Cell has no parent grid");

            HighlightController ??= GetComponentInChildren<MaterialHighlightable>();
            if (!HighlightController) Debug.LogError("Cell is missing a MaterialHighlightable");
        }


        public void Setup(
            Vector2Int coordinates,
            ECellHeight cellHeight,
            TerrainTypeSO terrainType,
            BiomeTypeSO biomeType
        )
        {
            Coordinates = coordinates;
            Height = cellHeight;
            TerrainType = terrainType;
            BiomeType = biomeType;
            
            _cellTerrainAccessibilityController = new CellTerrainAccessibilityController(terrainType);
            _cellVisualController = new CellVisualController(transform);
            _cellPositionController = new CellPositionController(
                transform,
                WorldHeightPerUnit,
                AHexGrid.CELL_SIZE,
                Coordinates
            );
            
            if (HighlightController)
                HighlightController.transform.localScale = Vector3.one * AHexGrid.CELL_SIZE / 2.894f;
            else
                Debug.LogError("Cell " + name + " has no visual to be set up. Please add a cell visual as a child.");
        }

        // ACCESSIBILITY        
        public void GenerateRandomAccessibility(EAccessibilityGenerationMode mode)
        {
            _cellTerrainAccessibilityController ??= new CellTerrainAccessibilityController(TerrainType);
            _cellTerrainAccessibilityController.GenerateRandomAccessibility(mode);
        }

        public void LoadAccessibility(bool accessibility)
        {
            _cellTerrainAccessibilityController ??= new CellTerrainAccessibilityController(TerrainType);
            _cellTerrainAccessibilityController.OverrideAccessibility(accessibility);
        }

        public void SetTerrain(TerrainTypeSO terrainType)
        {
            TerrainType = terrainType;
            _cellVisualController?.ApplyVisuals(terrainType, IsAccessible);
        }

        // POSITION
        public Vector3 GetCenter()
        {
            return _cellPositionController.GetCenter(Height);
        }

        public float GetYPosition()
        {
            return _cellPositionController.GetYPosition(Height);
        }

        public void UpdateHeight(ECellHeight newHeight, float duration)
        {
            Height = newHeight;
            if (duration <= 0f)
                _cellPositionController.SetInstanceHeight(newHeight);
            else
                StartCoroutine(_cellPositionController.SmoothMoveToHeight(newHeight, duration));
        }

        public static Vector2Int GetHexDirection(Cell initiatorCell, Cell targetCell)
        {
            Vector2Int initiatorAxial = initiatorCell.AxialCoordinates;
            Vector2Int targetAxial = targetCell.AxialCoordinates;
            return targetAxial - initiatorAxial;
        }

        // VISUALS
        public void DestroyVisual()
        {
            _cellVisualController.DestroyVisual();
        }

        public void LoadVisual(
            TerrainTypeSO terrainType,
            string visualPrefabPath,
            Vector3 visualPosition,
            Quaternion visualRotation
        )
        {
            _cellVisualController.RecoverVisuals(terrainType, visualPrefabPath, visualPosition, visualRotation);
        }

        public void UpdateBaseCellMaterial()
        {
            _cellVisualController.SetBaseCellMaterial(TerrainType);
        }

        #region toString
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
                   $"- HighlightController: {(HighlightController != null ? "Set" : "Not Set")}\n";
        }
        #endregion
        
        #region abstract methods
        public abstract bool IsTerrainAccessible();
        public abstract bool IsFree();
        #endregion
    }
}