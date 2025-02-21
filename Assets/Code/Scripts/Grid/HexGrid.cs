using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Represents a hexagonal grid in the game.
    /// Handles the initialization, generation, and management of grid cells.
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        [field: SerializeField, Header("Grid settings"), Range(000_001, 999_999)] public int Width { get; set; }
        [field: SerializeField, Range(000_001, 999_999)] public int Height { get; set; }
        [field: SerializeField] public float HexSize { get; set; } = 2f;
        [field: SerializeField] public GameObject HexPrefab { get; set; }
        [field: SerializeField, Header("Biomes's list")] public BiomeTypeSO[] AvailableBiomes { get; set; }
        [field: SerializeField, Header("Procedural generation settings"), Range(0.1f, 0.99f)] public float NoiseScale { get; set; } = 0.2f;
        [field: SerializeField, Range(000_000_000, 999_999_999)] public int Seed { get; set; } = 000_000_000;
        [field: SerializeField] public Dictionary<Vector2Int, Cell> Cells { get; set; } = new();
        private IGridGenerator _gridGenerator;

        /// <summary>
        /// Called when the script is loaded. Initializes the grid by calling <see cref="Initialize"/>.
        /// </summary>
        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the grid by validating its dimensions and setting up the grid generator.
        /// </summary>
        /// <remarks>
        /// If the grid dimensions are invalid, an error message is logged and the grid is not generated.
        /// </remarks>
        public void Initialize()
        {
            _gridGenerator = GridFactory.CreateGridGenerator(EGridType.KINGDOM, Width, Height, AvailableBiomes, this.transform, NoiseScale, Seed);
            ProcessGeneration();
        }

        /// <summary>
        /// Retrieves all the <see cref="Cell"/> components that are children of the current GameObject.
        /// </summary>
        /// <returns>An array of <see cref="Cell"/> components found in the children of the current GameObject.</returns>
        public Cell[] GetCells()
        {
            return GetComponentsInChildren<Cell>();
        }

        /// <summary>
        /// Processes the generation of the grid by clearing any existing cells and generating new ones.
        /// </summary>
        private void ProcessGeneration()
        {
            ClearCells();
            Cells = _gridGenerator.GenerateGrid(HexPrefab.GetComponent<Cell>(), HexSize);
        }

        /// <summary>
        /// Clears all the cells from the grid by destroying their corresponding game objects.
        /// </summary>
        public void ClearCells()
        {
            foreach (Cell child in GetCells())
            {
                DestroyImmediate(child.gameObject);
            }
            Cells?.Clear();
        }

        /// <summary>
        /// Generates random terrain heights for each cell based on predefined rules.
        /// Cells of water terrain will be given a low height, others will have a random height.
        /// </summary>
        public void GenerateRandomHeight()
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Cell cell = transform.GetChild(i).GetComponent<Cell>();
                ECellHeight randomCellHeight;
                if (cell.TerrainType.name.Contains("Water"))
                {
                    randomCellHeight = ECellHeight.LOW;
                }
                else
                {
                    randomCellHeight = (ECellHeight)Randomizer.GetRandomIntBetween(-1, 2);
                }
                cell.UpdateHeight(randomCellHeight, 0);
            }
        }

        /// <summary>
        /// Returns a string representation of the grid's current state, including its dimensions, hex size, and number of cells.
        /// </summary>
        /// <returns>A string describing the grid's properties and the total number of cells.</returns>
        public override string ToString()
        {
            return $"HexGrid:\n" +
                    $"- Width: {Width}\n" +
                    $"- Height: {Height}\n" +
                    $"- HexSize: {HexSize}\n" +
                    $"- HexPrefab: {(HexPrefab != null ? HexPrefab.name : "None")}\n" +
                    $"- Noise Scale: {NoiseScale}\n" +
                    $"- Seed: {Seed}\n" +
                    $"- Available Biomes: {(AvailableBiomes != null && AvailableBiomes.Length > 0 ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                    $"- Total Cells: {Cells?.Count ?? 0}";
        }
    }
}
