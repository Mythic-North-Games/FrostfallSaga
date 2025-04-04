using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public abstract class ABaseGridGenerator
    {
        protected const float HexSize = 2.0f;

        protected ABaseGridGenerator()
        {
        }


        protected ABaseGridGenerator(Cell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes,
            Transform parentGrid, float noiseScale, int seed)
        {
            HexPrefab = hexPrefab;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            AvailableBiomes = availableBiomes;
            ParentGrid = parentGrid;
            NoiseScale = noiseScale;
            Seed = seed;
        }

        protected ABaseGridGenerator(Cell hexPrefab, int gridWidth, int gridHeight, Transform parentGrid,
            float noiseScale, int seed)
        {
            HexPrefab = hexPrefab;
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            ParentGrid = parentGrid;
            NoiseScale = noiseScale;
            Seed = seed;
        }

        protected int GridWidth { get; }
        protected int GridHeight { get; }
        protected BiomeTypeSO[] AvailableBiomes { get; }
        protected Transform ParentGrid { get; }
        protected float NoiseScale { get; set; }
        protected int Seed { get; set; }
        protected Cell HexPrefab { get; set; }
        public abstract Dictionary<Vector2Int, Cell> GenerateGrid();

        public override string ToString()
        {
            return "BaseGridGenerator:\n" +
                   $"- GridWidth: {GridWidth}\n" +
                   $"- GridHeight: {GridHeight}\n" +
                   $"- Available Biomes: {(AvailableBiomes is { Length: > 0 } ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                   $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                   $"- NoiseScale: {NoiseScale}\n" +
                   $"- Seed: {Seed}";
        }
    }
}