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
        private readonly TerrainTypeSO _defaultTerrainType =
            Resources.Load<TerrainTypeSO>("ScriptableObjects/Grid/Terrain/TerrainTypeDarkForest");

        private readonly Dictionary<HexDirection, Cell> _hexDirectionCells;

        private readonly PerlinTerrainManager _perlinTerrainManager;

        public FightGridGenerator(FightCell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes,
            Transform parentGrid, float noiseScale, int seed)
            : base(hexPrefab, gridWidth, gridHeight, availableBiomes, parentGrid, noiseScale, seed)
        {
            _perlinTerrainManager = new PerlinTerrainManager(noiseScale, seed);
            _hexDirectionCells = GameStateManager.Instance.GetPreFightData().HexDirectionCells;
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid()
        {
            TerrainTypeSO[] validTerrainTypes = GetValidTerrainTypes();
            Dictionary<Vector2Int, Cell> gridCells = new();
            Vector2Int centerCoords = new(GridWidth / 2, GridHeight / 2);

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexSize, x, y);
                    Cell cell = Object.Instantiate(HexPrefab, centerPosition, Quaternion.identity, ParentGrid);
                    cell.name = $"Cell[{x};{y}]";
                    HexDirection section = DetermineSection(x, y, centerCoords);
                    Debug.Log("Section : " + section);
                    TerrainTypeSO selectedTerrain = SelectTerrainType(section, validTerrainTypes);
                    Debug.Log("Selected terrain : " + selectedTerrain);
                    BiomeTypeSO selectedBiome =
                        AvailableBiomes.FirstOrDefault(biome => biome.TerrainTypeSO.Contains(selectedTerrain));
                    SetupCell(cell, x, y, selectedBiome, HexSize, selectedTerrain);
                    gridCells[new Vector2Int(x, y)] = cell;
                }
            }

            GenerateHighByFromPerlinNoise(gridCells);
            return gridCells;
        }

        private TerrainTypeSO SelectTerrainType(HexDirection section, TerrainTypeSO[] validTerrainTypes)
        {
            //FIXME PB avec les valeurs null en bord de terrain. (fix potentiel : au lieu de null => cell default aet set un terrain aléatoire cohérent)
            if (_hexDirectionCells.TryGetValue(section, out Cell cell) && cell.TerrainType != null)
            {
                return cell.TerrainType;
            }

            if (validTerrainTypes.Length > 0)
            {
                return Randomizer.GetRandomElementFromArray(validTerrainTypes);
            }

            return _defaultTerrainType;
        }

        private TerrainTypeSO[] GetValidTerrainTypes()
        {
            TerrainTypeSO[] validTerrainTypes = _hexDirectionCells
                .Where(keyValuePair => keyValuePair.Value != null && keyValuePair.Value.TerrainType != null)
                .Select(keyValuePair => keyValuePair.Value.TerrainType)
                .Distinct()
                .ToArray();
            Debug.Log(validTerrainTypes.Length);
            return validTerrainTypes;
        }

        private void SetupCell(Cell cell, int x, int y, BiomeTypeSO selectedBiome, float hexSize,
            TerrainTypeSO selectedTerrain)
        {
            Debug.Log($"Selected terrain : " + selectedTerrain.name);
            cell.Setup(new Vector2Int(x, y), ECellHeight.LOW, hexSize, selectedTerrain, selectedBiome);
            cell.HighlightController.SetupInitialMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.UpdateCurrentDefaultMaterial(selectedTerrain.CellMaterial);
            cell.HighlightController.ResetToDefaultMaterial();
        }

        private HexDirection DetermineSection(int x, int y, Vector2Int center)
        {
            int dx = x - center.x;
            int dy = y - center.y;

            return dx switch
            {
                < 0 when dy == 0 => HexDirection.WEST,
                < 0 when dy > 0 => HexDirection.NORTHWEST,
                >= 0 when dy > 0 => HexDirection.NORTHEAST,
                > 0 when dy == 0 => HexDirection.EAST,
                > 0 when dy < 0 => HexDirection.SOUTHEAST,
                <= 0 when dy < 0 => HexDirection.SOUTHWEST,
                _ => HexDirection.WEST
            };
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
            return "BaseGridGenerator:\n" +
                   $"- GridWidth: {GridWidth}\n" +
                   $"- GridHeight: {GridHeight}\n" +
                   $"- Available Biomes: {(AvailableBiomes is { Length: > 0 } ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                   $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                   $"- NoiseScale: {(NoiseScale.HasValue ? NoiseScale.Value.ToString() : "None")}\n" +
                   $"- Seed: {Seed?.ToString() ?? "None"}";
        }
    }
}