using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Procedural;
using FrostfallSaga.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrostfallSaga.Dungeon
{
    public class DungeonGridGenerator : ABaseGridGenerator
    {
        private readonly BiomeTypeSO _defaultBiomeType;
        private BiomeTypeSO _biomeType;
        private PerlinTerrainManager _perlinTerrainManager;

        public DungeonGridGenerator(FightCell hexPrefab, int gridWidth, int gridHeight,
            Transform parentGrid, float noiseScale, int seed, BiomeTypeSO defaultBiome)
            : base(hexPrefab, gridWidth, gridHeight, parentGrid, noiseScale, seed)
        {
            _biomeType = GameStateManager.Instance.GetDungeonState().DungeonConfiguration.BiomeType;
            _perlinTerrainManager = new PerlinTerrainManager(noiseScale, seed);
            _defaultBiomeType = defaultBiome;
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid()
        {
            Dictionary<Vector2Int, Cell> gridCells = new();
            if (!_biomeType || _biomeType.TerrainTypeSO.Length == 0)
            {
                Debug.Log("No biome or terrain types available in dungeon configuration.");
                _biomeType = _defaultBiomeType;
                if (!_biomeType || _biomeType.TerrainTypeSO.Length == 0)
                {
                    Debug.LogError("No biome or terrain types available in default configuration.");
                    return null;
                }
            }

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexSize, x, y);
                    Cell cell = Object.Instantiate(HexPrefab, centerPosition, Quaternion.identity, ParentGrid);
                    cell.name = $"Cell[{x};{y}]";
                    float perlinValue = _perlinTerrainManager.GetNoiseValue(x, y);
                    TerrainTypeSO selectedTerrain = GetTerrainTypeFromPerlinValue(perlinValue, _biomeType);
                    SetupCell(cell, x, y, _biomeType, HexSize, selectedTerrain);
                    gridCells[new Vector2Int(x, y)] = cell;
                }
            }

            GenerateHighByFromPerlinNoise(gridCells);
            return gridCells;
        }


        private static void SetupCell(Cell cell, int x, int y, BiomeTypeSO selectedBiome, float hexSize,
            TerrainTypeSO selectedTerrain)
        {
            cell.Setup(new Vector2Int(x, y), ECellHeight.LOW, hexSize, selectedTerrain, selectedBiome);
            cell.GenerateRandomAccessibility(EAccessibilityGenerationMode.FLIP_IF_ACCESSIBLE);
            cell.SetTerrain(selectedTerrain);
            cell.HighlightController.SetupInitialMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.UpdateCurrentDefaultMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.ResetToDefaultMaterial();
        }

        private static TerrainTypeSO GetTerrainTypeFromPerlinValue(float perlinValue, BiomeTypeSO selectedBiome)
        {
            TerrainTypeSO[] availableTerrains = selectedBiome.TerrainTypeSO;

            int terrainCount = availableTerrains.Length;
            float segmentSize = 1f / terrainCount;

            for (int i = 0; i < terrainCount; i++)
            {
                if (perlinValue < (i + 1) * segmentSize)
                {
                    return availableTerrains[i];
                }
            }

            return availableTerrains[terrainCount - 1];
        }

        private void GenerateHighByFromPerlinNoise(Dictionary<Vector2Int, Cell> grid)
        {
            _perlinTerrainManager =
                new PerlinTerrainManager(NoiseScale, Randomizer.GetRandomIntBetween(000_000_000, 999_999_999));
            ECellHeight[] heights = (ECellHeight[])Enum.GetValues(typeof(ECellHeight));
            float segmentSize = 1f / heights.Length;
            foreach (KeyValuePair<Vector2Int, Cell> cell in grid)
            {
                float perlinValue = _perlinTerrainManager.GetNoiseValue(cell.Key.x, cell.Key.y);

                for (int i = 0; i < heights.Length; i++)
                {
                    if (perlinValue < (i + 1) * segmentSize)
                    {
                        cell.Value.UpdateHeight(heights[i], 0f);
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return "FightGridGenerator:\n" +
                   $"- GridWidth: {GridWidth}\n" +
                   $"- GridHeight: {GridHeight}\n" +
                   $"- Available Biomes: {(AvailableBiomes is { Length: > 0 } ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                   $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                   $"- NoiseScale: {NoiseScale}\n" +
                   $"- Seed: {Seed}\n";
        }
    }
}