using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public static class CellAnalysis
    {
        public static Cell TargetCell { get; private set; }
        public static Cell[] NeighborCells { get; private set; }
        public static Dictionary<TerrainTypeSO, int> TerrainCount { get; private set; }
        public static Dictionary<TerrainTypeSO, float> TerrainComposition { get; private set; }
        public static Dictionary<BiomeTypeSO, int> BiomeCount { get; private set; }
        private static float _totalCells = 0;


        public static void AnalyzeAtCell(Cell targetCell, HexGrid grid)
        {
            TargetCell = targetCell;
            NeighborCells = CellsNeighbors.GetNeighbors(grid, targetCell);
            TerrainCount = new Dictionary<TerrainTypeSO, int>();
            TerrainComposition = new Dictionary<TerrainTypeSO, float>();
            BiomeCount = new Dictionary<BiomeTypeSO, int>();
            _totalCells = NeighborCells.Count() + 1;

            CountTerrain(TargetCell.TerrainType);
            CountBiome(TargetCell.BiomeType);

            foreach (Cell neighbor in NeighborCells)
            {
                CountTerrain(neighbor.TerrainType);
                CountBiome(neighbor.BiomeType);
            }
        }

        private static void CountTerrain(TerrainTypeSO terrain)
        {
            if (TerrainCount.ContainsKey(terrain))
            {
                TerrainCount[terrain]++;
            }
            else
            {
                TerrainCount[terrain] = 1;
            }
        }

        private static void CountBiome(BiomeTypeSO biome)
        {
            if (BiomeCount.ContainsKey(biome))
            {
                BiomeCount[biome]++;
            }
            else
            {
                BiomeCount[biome] = 1;
            }
        }

        private static Dictionary<TerrainTypeSO, float> GetTerrainPercentages()
        {
            return TerrainCount.ToDictionary(terrain => terrain.Key, terrain => terrain.Value / _totalCells * 100);
        }

        private static Dictionary<BiomeTypeSO, float> GetBiomePercentages()
        {
            return BiomeCount.ToDictionary(biome => biome.Key, biome => biome.Value / _totalCells * 100);
        }

        public static void PrintAnalysis()
        {
            Debug.Log($"Analysis for Cell at {TargetCell.Coordinates}:");

            Debug.Log("Terrains:");
            foreach (var terrain in TerrainCount)
                Debug.Log($"{terrain.Key}: {terrain.Value}");

            Debug.Log("Biomes:");
            foreach (var biome in BiomeCount)
                Debug.Log($"{biome.Key}: {biome.Value}");
        }

        public static void PrintAnalysisWithPercentages()
        {
            Debug.Log($"Analysis for Cell at {TargetCell.Coordinates}:");

            Debug.Log("Terrain Percentages:");
            foreach (var terrain in GetTerrainPercentages())
                Debug.Log($"{terrain.Key}: {terrain.Value:F2}%");

            Debug.Log("Biome Percentages:");
            foreach (var biome in GetBiomePercentages())
                Debug.Log($"{biome.Key}: {biome.Value:F2}%");
        }
    }

}
