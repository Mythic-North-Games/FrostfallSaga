using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.KingdomToFight
{
    public class CellAnalysis
    {
        public Cell TargetCell { get; private set; }
        public Cell[] NeighborCells { get; private set; }
        public Dictionary<TerrainTypeSO, int> TerrainCount { get; private set; }
        public Dictionary<BiomeTypeSO, int> BiomeCount { get; private set; }
        private float _totalCells = 0;

        public CellAnalysis(Cell targetCell, HexGrid grid)
        {
            TargetCell = targetCell;
            NeighborCells = CellsNeighbors.GetNeighbors(grid, targetCell);
            TerrainCount = new Dictionary<TerrainTypeSO, int>();
            BiomeCount = new Dictionary<BiomeTypeSO, int>();
            _totalCells = NeighborCells.Count() + 1;
        }

        public void Analyze()
        {
            CountTerrain(TargetCell.TerrainType);
            CountBiome(TargetCell.BiomeType);

            foreach (var neighbor in NeighborCells)
            {
                CountTerrain(neighbor.TerrainType);
                CountBiome(neighbor.BiomeType);
            }
        }

        private void CountTerrain(TerrainTypeSO terrain)
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

        private void CountBiome(BiomeTypeSO biome)
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

        private Dictionary<TerrainTypeSO, float> GetTerrainPercentages()
        {
            return TerrainCount.ToDictionary(terrain => terrain.Key, terrain => terrain.Value / _totalCells * 100);
        }

        private Dictionary<BiomeTypeSO, float> GetBiomePercentages()
        {
            return BiomeCount.ToDictionary(biome => biome.Key, biome => biome.Value / _totalCells * 100);
        }

        public void PrintAnalysis()
        {
            Debug.Log($"Analysis for Cell at {TargetCell.Coordinates}:");

            Debug.Log("Terrains:");
            foreach (var terrain in TerrainCount)
                Debug.Log($"{terrain.Key}: {terrain.Value}");

            Debug.Log("Biomes:");
            foreach (var biome in BiomeCount)
                Debug.Log($"{biome.Key}: {biome.Value}");
        }

        public void PrintAnalysisWithPercentages()
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
