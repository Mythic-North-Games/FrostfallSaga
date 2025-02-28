using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public abstract class ABaseGridGenerator
    {
        protected int GridWidth { get; }
        protected int GridHeight { get; }
        protected BiomeTypeSO[] AvailableBiomes { get; }
        protected Transform ParentGrid {  get; }
        protected float? NoiseScale { get; set; }
        protected int? Seed { get; set; }
        protected Cell HexPrefab { get; set; }
        protected float HexSize = 2.0f;
        protected GameObject[] InterestPoints {  get; set; }

        protected ABaseGridGenerator() { }

        protected ABaseGridGenerator(Cell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid) 
        { 
            HexPrefab = hexPrefab;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            AvailableBiomes = availableBiomes;
            ParentGrid = parentGrid;
        }
        protected ABaseGridGenerator(Cell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid, float noiseScale, int seed)
        {
            HexPrefab = hexPrefab;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            AvailableBiomes = availableBiomes;
            ParentGrid = parentGrid;
            NoiseScale = noiseScale;
            Seed = seed;
        }
        protected ABaseGridGenerator(Cell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid, float noiseScale, int seed, GameObject[] interestPoints)
        {
            HexPrefab = hexPrefab;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            AvailableBiomes = availableBiomes;
            ParentGrid = parentGrid;
            NoiseScale = noiseScale;
            Seed = seed;
            InterestPoints = interestPoints;
        }
        public abstract Dictionary<Vector2Int, Cell> GeneratorGenerateGrid();

        public override string ToString()
        {
            return  $"BaseGridGenerator:\n" +
                    $"- GridWidth: {GridWidth}\n" +
                    $"- GridHeight: {GridHeight}\n" +
                    $"- Available Biomes: {(AvailableBiomes != null && AvailableBiomes.Length > 0 ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                    $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                    $"- NoiseScale: {(NoiseScale.HasValue ? NoiseScale.Value.ToString() : "None")}\n" +
                    $"- Seed: {Seed?.ToString() ?? "None"}";
        }
    }
}
