using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Procedural;
using FrostfallSaga.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrostfallSaga.Grid
{
    public class FightGridGenerator : ABaseGridGenerator
    {

        private readonly Dictionary<HexDirection, Cell> _hexDirectionCells;
        private readonly PerlinTerrainManager _perlinTerrainManager;
        private readonly TerrainTypeSO _defaultTerrainType;

        public FightGridGenerator(FightCell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes,
            Transform parentGrid, float noiseScale, int seed, TerrainTypeSO defaultTerrainType)
            : base(hexPrefab, gridWidth, gridHeight, availableBiomes, parentGrid, noiseScale, seed)
        {
            _perlinTerrainManager = new PerlinTerrainManager(noiseScale, seed);
            _hexDirectionCells = GameStateManager.Instance.GetPreFightData().HexDirectionCells;
            _defaultTerrainType = defaultTerrainType;
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid()
        {
            Dictionary<Vector2Int, Cell> gridCells = new();
            Vector2Int centerCoords = new(GridWidth / 2, GridHeight / 2);

            Dictionary<HexDirection, TerrainTypeSO> terrainByDirection = BuildTerrainMapForDirections();

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexSize, x, y);
                    Cell cell = Object.Instantiate(HexPrefab, centerPosition, Quaternion.identity, ParentGrid);
                    cell.name = $"Cell[{x};{y}]";

                    HexDirection section = DetermineSection(x, y, centerCoords);
                    TerrainTypeSO selectedTerrain = terrainByDirection[section];
                    BiomeTypeSO selectedBiome = AvailableBiomes
                        .FirstOrDefault(b => b.TerrainTypeSO.Contains(selectedTerrain));

                    SetupCell(cell, x, y, selectedBiome, HexSize, selectedTerrain);
                    gridCells[new Vector2Int(x, y)] = cell;
                }
            }

            GenerateHighByFromPerlinNoise(gridCells);
            return gridCells;
        }


        private static void SetupCell(Cell cell, int x, int y, BiomeTypeSO selectedBiome, float hexSize,
            TerrainTypeSO selectedTerrain)
        {
            float chance = selectedTerrain.IsAccessible ? 1f : selectedTerrain.AccessibilityChanceOverride;
            bool isAccessible = Randomizer.GetBooleanOnChance(chance);
            cell.ForceAccessibility(isAccessible);
            cell.Setup(new Vector2Int(x, y), ECellHeight.LOW, hexSize, selectedTerrain, selectedBiome);
            cell.HighlightController.SetupInitialMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.UpdateCurrentDefaultMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.ResetToDefaultMaterial();
        }

        private static HexDirection DetermineSection(int x, int y, Vector2Int center)
        {
            Vector2 delta = new(x - center.x, y - center.y);
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            angle = (angle + 360) % 360;

            if (angle >= 330 || angle < 30) return HexDirection.EAST;
            if (angle >= 30 && angle < 90) return HexDirection.NORTHEAST;
            if (angle >= 90 && angle < 150) return HexDirection.NORTHWEST;
            if (angle >= 150 && angle < 210) return HexDirection.WEST;
            if (angle >= 210 && angle < 270) return HexDirection.SOUTHWEST;
            return HexDirection.SOUTHEAST;
        }

        private Dictionary<HexDirection, TerrainTypeSO> BuildTerrainMapForDirections()
        {
            Dictionary<HexDirection, TerrainTypeSO> terrainByDirection = new();

            foreach (HexDirection dir in Enum.GetValues(typeof(HexDirection)))
            {
                if (_hexDirectionCells.TryGetValue(dir, out var cell) && cell?.TerrainType != null)
                {
                    terrainByDirection[dir] = cell.TerrainType;
                }
            }

            List<TerrainTypeSO> knownTerrains = terrainByDirection.Values.Distinct().ToList();

            foreach (HexDirection dir in Enum.GetValues(typeof(HexDirection)))
            {
                if (!terrainByDirection.ContainsKey(dir))
                {
                    terrainByDirection[dir] = knownTerrains.Count > 0
                        ? Randomizer.GetRandomElementFromList(knownTerrains)
                        : _defaultTerrainType;
                }
            }

            return terrainByDirection;
        }


        private void GenerateHighByFromPerlinNoise(Dictionary<Vector2Int, Cell> grid)
        {
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
            string baseInfo = "FightGridGenerator:\n" +
                              $"- GridWidth: {GridWidth}\n" +
                              $"- GridHeight: {GridHeight}\n" +
                              $"- Available Biomes: {(AvailableBiomes is { Length: > 0 } ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                              $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                              $"- NoiseScale: {(NoiseScale.HasValue ? NoiseScale.Value.ToString("F2") : "None")}\n" +
                              $"- Seed: {Seed?.ToString() ?? "None"}\n";

            string terrainMap = "- Terrain mapping by HexDirection:\n";

            Dictionary<HexDirection, TerrainTypeSO> terrainByDirection = BuildTerrainMapForDirections();

            foreach (HexDirection dir in Enum.GetValues(typeof(HexDirection)))
            {
                if (terrainByDirection.TryGetValue(dir, out TerrainTypeSO terrain))
                {
                    terrainMap += $"  • {dir}: {terrain.TypeName} (Accessible: {terrain.IsAccessible}, Chance: {terrain.AccessibilityChanceOverride:P0})\n";
                }
                else
                {
                    terrainMap += $"  • {dir}: No terrain mapped\n";
                }
            }

            return baseInfo + terrainMap;
        }
    }
}