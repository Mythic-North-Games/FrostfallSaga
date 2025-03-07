using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Procedural;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class FightGridGenerator : ABaseGridGenerator
    {
        private static readonly BiomeTypeSO DefaultBiome =
            Resources.Load<BiomeTypeSO>("EditModeTests/ScriptableObjects/TestBiome");

        private readonly PerlinTerrainManager _perlinTerrainManager;

        public FightGridGenerator(FightCell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes,
            Transform parentGrid, float noiseScale, int seed)
            : base(hexPrefab, gridWidth, gridHeight, availableBiomes, parentGrid, noiseScale, seed)
        {
            _perlinTerrainManager = new PerlinTerrainManager(noiseScale, seed);
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid()
        {
            Dictionary<Vector2Int, Cell> gridCells = new();

            for (int y = 0; y < GridHeight; y++)
            for (int x = 0; x < GridWidth; x++)
            {
                Vector3 centerPosition = HexMetrics.Center(HexSize, x, y);
                Cell cell = Object.Instantiate(HexPrefab, centerPosition, Quaternion.identity, ParentGrid);
                cell.name = $"Cell[{x};{y}]";
                SetupCell(cell, x, y, DefaultBiome, HexSize);
                gridCells[new Vector2Int(x, y)] = cell;
            }

            return gridCells;
        }

        private void SetupCell(Cell cell, int x, int y, BiomeTypeSO selectedBiome, float hexSize)
        {
            float perlinValue = _perlinTerrainManager.GetNoiseValue(x, y);
            TerrainTypeSO selectedTerrain = GetTerrainTypeFromPerlinValue(perlinValue, selectedBiome);
            cell.Setup(new Vector2Int(x, y), ECellHeight.LOW, hexSize, selectedTerrain, selectedBiome);
            cell.HighlightController.SetupInitialMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.UpdateCurrentDefaultMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.ResetToDefaultMaterial();
        }

        private TerrainTypeSO GetTerrainTypeFromPerlinValue(float perlinValue, BiomeTypeSO selectedBiome)
        {
            TerrainTypeSO[] availableTerrains = selectedBiome.TerrainTypeSO;

            if (availableTerrains == null || availableTerrains.Length == 0)
            {
                Debug.LogError("No terrain types available for the current biome.");
                return null;
            }

            int terrainCount = availableTerrains.Length;
            float segmentSize = 1f / terrainCount;

            for (int i = 0; i < terrainCount; i++)
                if (perlinValue < (i + 1) * segmentSize)
                    return availableTerrains[i];

            return availableTerrains[terrainCount - 1];
        }

        public override string ToString()
        {
            return "BaseGridGenerator:\n" +
                   $"- GridWidth: {GridWidth}\n" +
                   $"- GridHeight: {GridHeight}\n" +
                   $"- DefaultBiome: {DefaultBiome}\n" +
                   $"- Available Biomes: {(AvailableBiomes != null && AvailableBiomes.Length > 0 ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                   $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                   $"- NoiseScale: {(NoiseScale.HasValue ? NoiseScale.Value.ToString() : "None")}\n" +
                   $"- Seed: {Seed?.ToString() ?? "None"}";
        }
    }
}