using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Core;
using FrostfallSaga.Procedural;
using Codice.CM.Common;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Expose methods for generating cells inside a given grid.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class GridCellsGenerator : MonoBehaviour
    {
        [field: SerializeField] public HexGrid HexGrid { get; private set; }
        [field: SerializeField] public Material AlternativeMaterial { get; private set; }
        [field: SerializeField, Header("Biome caracteristics"), Tooltip("Biome's type")] public BiomeTypeSO BiomeType { get; private set; }
        [field: SerializeField, Header("Perlin Noise"), Tooltip("Statistics about Perlin Noise")] public PerlinNoiseGenerator PerlinNoiseGenerator {get; private set;}
        private float lastSeed;

        private void Awake()
        {
            if (HexGrid == null)
            {
                HexGrid = GetComponentInParent<HexGrid>();
            }
            if (HexGrid == null)
            {
                Debug.LogError("HexGridGenerator could not find HexGrid component in its parent or itself");
            }
            PerlinNoiseGenerator = new PerlinNoiseGenerator(PerlinNoiseGenerator.noiseScale, PerlinNoiseGenerator.seed);
            lastSeed = PerlinNoiseGenerator.seed;
        }

        /// <summary>
        /// Clears existing cells of the grid if any then generate new ones.
        /// </summary>
        public void GenerateCells()
        {
            ClearCells();

            if (PerlinNoiseGenerator.seed != lastSeed)
            {
                PerlinNoiseGenerator = new PerlinNoiseGenerator(PerlinNoiseGenerator.noiseScale, PerlinNoiseGenerator.seed);
                lastSeed = PerlinNoiseGenerator.seed;
            }

            Quaternion rotation = Quaternion.identity;
            if (HexGrid.HexOrientation.Equals(ECellOrientation.FlatTop))
            {
                rotation = Quaternion.Euler(new Vector3(0f, -30f, 0f));
            }

            for (int z = 0; z < HexGrid.Height; z++)
            {
                for (int x = 0; x < HexGrid.Width; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexGrid.HexSize, x, z, HexGrid.HexOrientation);
                    GameObject newHex = Instantiate(HexGrid.HexPrefab, centerPosition, rotation, HexGrid.transform);
                    SetupCellForInstanciatedCellPrefab(newHex, x, z);
                }
            }
            HexGrid.FindAndSetCellsByCoordinates();
        }

        /// <summary>
        /// Updates the height of existing cells with a random height.
        /// </summary>
        public void GenerateRandomHeight()
        {
            int childCount = HexGrid.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Cell cell = HexGrid.transform.GetChild(i).GetComponent<Cell>();
                ECellHeight randomCellHeight;
                if (cell.TerrainType.name.Contains("Water"))
                {
                    randomCellHeight = ECellHeight.LOW;
                }
                else
                {
                    randomCellHeight = (ECellHeight) Randomizer.GetRandomIntBetween(-1, 2);
                }
                cell.UpdateHeight(randomCellHeight);
            }
        }

        private void SetupCellForInstanciatedCellPrefab(GameObject cellPrefab, int x, int z)
        {
            cellPrefab.transform.name = "Cell[" + x + ";" + z + "]";
            Cell newCell = cellPrefab.GetComponent<Cell>();

            float perlinValue = PerlinNoiseGenerator.GetNoiseValue(x, z);
            TerrainTypeSO terrainType = GetTerrainTypeFromPerlinValue(perlinValue);

            newCell.Setup(new Vector2Int(x, z), ECellHeight.LOW, HexGrid.HexSize, terrainType);
            newCell.HighlightController.SetupInitialMaterial(terrainType.CellMaterial);
            newCell.HighlightController.UpdateCurrentDefaultMaterial(terrainType.CellMaterial);
            newCell.HighlightController.ResetToInitialMaterial();
        }

        /// <summary>
        /// Define with the perlinValue wich Terrain will be choose
        /// </summary>
        /// <param name="perlinValue">float value will determine wich terrain type</param>
        /// <returns></returns>
        private TerrainTypeSO GetTerrainTypeFromPerlinValue(float perlinValue)
        {
            TerrainTypeSO[] _availableTerrains = BiomeType.TerrainTypeSO;

            if (_availableTerrains == null || _availableTerrains.Length == 0)
            {
                Debug.LogError("No terrain types available for the current biome.");
                return null;
            }

            int _terrainCount = _availableTerrains.Length;
            float _segmentSize = 1f / _terrainCount;

            for (int i = 0; i < _terrainCount; i++)
            {
                if (perlinValue < (i + 1) * _segmentSize)
                {
                    return _availableTerrains[i];
                }
            }

            return _availableTerrains[_terrainCount - 1];
        }

        /// <summary>
        /// Destroys all the cells of the grid.
        /// </summary>
        public void ClearCells()
        {

            GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
            if (cells != null)
            {
                foreach (GameObject cell in cells)
                {
                    DestroyImmediate(cell);
                }
            }
        }
    }
}
