using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public static class CellAnalysis
    {
        public static Dictionary<HexDirection, Cell> CellsByDirection = new();
        private static float _totalCells;
        public static Cell TargetCell { get; private set; }
        public static Cell[] NeighborCells { get; private set; }
        public static Dictionary<TerrainTypeSO, int> TerrainCount { get; private set; }
        public static Dictionary<TerrainTypeSO, float> TerrainComposition { get; private set; }
        public static Dictionary<BiomeTypeSO, int> BiomeCount { get; private set; }


        public static void AnalyzeAtCell(Cell targetCell, AHexGrid grid)
        {
            Debug.Log(grid.ToString());
            TargetCell = targetCell;
            NeighborCells = CellsNeighbors.GetNeighbors(grid, targetCell, includeHeightInaccessibleNeighbors: true,
                includeOccupiedNeighbors: true, includeInaccessibleNeighbors: true);
            TerrainCount = new Dictionary<TerrainTypeSO, int>();
            TerrainComposition = new Dictionary<TerrainTypeSO, float>();
            BiomeCount = new Dictionary<BiomeTypeSO, int>();
            _totalCells = NeighborCells.Count() + 1;
            Debug.Log(string.Format("TOTAL CELLS : {0}", _totalCells));

            CountTerrain(TargetCell.TerrainType);
            CountBiome(TargetCell.BiomeType);

            HexDirection[] direction =
            {
                HexDirection.NORTHWEST, HexDirection.NORTHEAST,
                HexDirection.WEST, HexDirection.SOUTHEAST,
                HexDirection.SOUTHWEST, HexDirection.WEST
            };

            for (var i = 0; i < NeighborCells.Length; i++)
            {
                Cell neighbor = NeighborCells[i];
                CountTerrain(neighbor.TerrainType);
                CountBiome(neighbor.BiomeType);
                CellsByDirection[direction[i]] = neighbor;
            }
        }

        private static void CountTerrain(TerrainTypeSO terrain)
        {
            if (TerrainCount.ContainsKey(terrain))
                TerrainCount[terrain]++;
            else
                TerrainCount[terrain] = 1;
        }

        private static void CountBiome(BiomeTypeSO biome)
        {
            if (BiomeCount.ContainsKey(biome))
                BiomeCount[biome]++;
            else
                BiomeCount[biome] = 1;
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
            foreach (KeyValuePair<TerrainTypeSO, int> terrain in TerrainCount)
                Debug.Log($"{terrain.Key}: {terrain.Value}");

            Debug.Log("Biomes:");
            foreach (KeyValuePair<BiomeTypeSO, int> biome in BiomeCount)
                Debug.Log($"{biome.Key}: {biome.Value}");
        }

        public static void PrintAnalysisWithPercentages()
        {
            Debug.Log($"Analysis for Cell at {TargetCell.Coordinates}:");

            Debug.Log("Terrain Percentages:");
            foreach (KeyValuePair<TerrainTypeSO, float> terrain in GetTerrainPercentages())
                Debug.Log($"{terrain.Key}: {terrain.Value:F2}%");

            Debug.Log("Biome Percentages:");
            foreach (KeyValuePair<BiomeTypeSO, float> biome in GetBiomePercentages())
                Debug.Log($"{biome.Key}: {biome.Value:F2}%");
        }

        public static void PrintAnalysisDict()
        {
            foreach (KeyValuePair<HexDirection, Cell> item in CellsByDirection)
                Debug.Log(string.Format("HexDirection : {0} || Cell name : {1} || Cell Terrain name : {2}", item.Key,
                    item.Value.name, item.Value.TerrainType.name));
        }
    }
}