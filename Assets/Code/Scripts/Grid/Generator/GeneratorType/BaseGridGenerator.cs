using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public abstract class BaseGridGenerator : IGridGenerator
    {
        protected int GridWidth { get; }
        protected int GridHeight { get; }
        protected BiomeTypeSO[] AvailableBiomes { get; }
        protected Transform ParentGrid {  get; }
        protected float? NoiseScale { get; set; }
        protected int? Seed { get; set; }

        protected BaseGridGenerator() { }

        protected BaseGridGenerator(int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid) 
        { 
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            AvailableBiomes = availableBiomes;
            ParentGrid = parentGrid;
        }
        protected BaseGridGenerator(int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid, float noiseScale, int seed)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            AvailableBiomes = availableBiomes;
            ParentGrid = parentGrid;
            NoiseScale = noiseScale;
            Seed = seed;
        }
        public abstract Dictionary<Vector2Int, Cell> GenerateGrid(Cell hexPrefab, float hexSize = 2.0f);

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
