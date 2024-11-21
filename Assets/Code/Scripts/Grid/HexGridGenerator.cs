using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Procedural;
using FrostfallSaga.Terrain;
using FrostfallSaga.Validator;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Manages the procedural generation of hexagonal grid cells, using Perlin and Voronoi noise to determine terrain and biome distribution.
    /// Responsible for initializing terrain and biome managers and setting up each cell's characteristics based on defined parameters.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider)), ExecuteAlways]
    public class GridCellsGenerator : MonoBehaviour
    {
        [Required]
        [field: SerializeField, Header("Default"), Tooltip("Default attributs required")] public HexGrid HexGrid { get; private set; }
        [Required]
        [field: SerializeField] public Material AlternativeMaterial { get; private set; }
        [Required]
        [field: SerializeField, Header("Biomes caracteristics"), Tooltip("Biomes's type")] public BiomeTypeSO[] AvailableBiomes { get; private set; }
        [Required]
        [field: SerializeField, Header("Generators managers"), Tooltip("Generation diffusion"), Range(0.1f, 0.99f)] public float NoiseScale { get; private set; } = 0.2f;
        [Required]
        [field: SerializeField, Tooltip("Generation reference"), Range(000_000_000, 999_999_999)] public int Seed { get; private set; } = 000_000_000;
        private PerlinTerrainManager _perlinTerrainManager;
        private VoronoiBiomeManager _voronoiBiomeManager;

        private int _lastSeed;

        /// <summary>
        /// Initializes components and verifies attributes on scene load to prepare for cell generation.
        /// </summary>
        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Clears any existing cells and generates new cells across the grid according to defined biomes and terrain characteristics.
        /// Uses Perlin noise for height variation and Voronoi noise for biome distribution.
        /// </summary>
        public void Initialize()
        {
            Validator.Validator.ValidateRequiredFields(this);
            
            _perlinTerrainManager = new PerlinTerrainManager(NoiseScale, Seed);
            _voronoiBiomeManager = new VoronoiBiomeManager(HexGrid.Width, HexGrid.Height, AvailableBiomes.Length, Seed);

            _lastSeed = Seed;
        }

        /// <summary>
        /// Checks if the list of available biomes is populated and valid, logging errors if null or empty.
        /// Returns true if all biomes are valid, false otherwise.
        /// </summary>
        public void GenerateCells()
        {
            ClearCells();

            if (!VerifyAvailableBiomesAttribut())
            {
                return;
            }

            if (Seed != _lastSeed)
            {
                _perlinTerrainManager = new PerlinTerrainManager(NoiseScale, Seed);
                _lastSeed = Seed;
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
                    int biomeIndex = _voronoiBiomeManager.GetClosestBiomeIndex(x, z);
                    BiomeTypeSO selectedBiome = AvailableBiomes[biomeIndex];
                    SetupCellForInstanciatedCellPrefab(newHex, x, z, selectedBiome);
                }
            }
            HexGrid.FindAndSetCellsByCoordinates();
        }

        /// <summary>
        /// Checks if the list of available biomes is populated and valid, logging errors if null or empty.
        /// Returns true if all biomes are valid, false otherwise.
        /// </summary>
        private bool VerifyAvailableBiomesAttribut()
        {
            if (AvailableBiomes == null || AvailableBiomes.Length == 0)
            {
                Debug.LogError("AvailableBiomes is null or empty. Cannot generate cells.");
                return false;
            }
            foreach (var biome in AvailableBiomes)
            {
                if (biome == null)
                {
                    Debug.LogError("One of the available biomes is null. Cannot generate cells.");
                    return false;
                }
            }
            return true;
        }

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
                    randomCellHeight = (ECellHeight)Randomizer.GetRandomIntBetween(-1, 2);
                }
                cell.UpdateHeight(randomCellHeight);
            }
        }

        private void SetupCellForInstanciatedCellPrefab(GameObject cellPrefab, int x, int z, BiomeTypeSO biome)
        {
            cellPrefab.transform.name = "Cell[" + x + ";" + z + "]";
            Cell newCell = cellPrefab.GetComponent<Cell>();
            float perlinValue = _perlinTerrainManager.GetNoiseValue(x, z);
            TerrainTypeSO terrainType = GetTerrainTypeFromPerlinValue(perlinValue, biome);
            newCell.Setup(new Vector2Int(x, z), ECellHeight.LOW, HexGrid.HexSize, terrainType, biome);
            newCell.HighlightController.SetupInitialMaterial(terrainType.CellMaterial);
            newCell.HighlightController.UpdateCurrentDefaultMaterial(terrainType.CellMaterial);
            newCell.HighlightController.ResetToInitialMaterial();
        }

        private TerrainTypeSO GetTerrainTypeFromPerlinValue(float perlinValue, BiomeTypeSO biome)
        {
            TerrainTypeSO[] availableTerrains = biome.TerrainTypeSO;

            if (availableTerrains == null || availableTerrains.Length == 0)
            {
                Debug.LogError("No terrain types available for the current biome.");
                return null;
            }

            int terrainCount = availableTerrains.Length;
            float segmentSize = 1f / terrainCount;

            for (int i = 0; i < terrainCount; i++)
            {
                if (perlinValue < (i + 1) * segmentSize)
                {
                    return availableTerrains[i];
                }
            }
            return availableTerrains[terrainCount]; // -1
        }

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