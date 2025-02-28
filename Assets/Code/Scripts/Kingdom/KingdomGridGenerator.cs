using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.InterestPoints;
using FrostfallSaga.Procedural;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class KingdomGridGenerator : ABaseGridGenerator
    {
        private VoronoiBiomeManager _voronoiBiomeManager;
        private PerlinTerrainManager _perlinTerrainManager;

        public KingdomGridGenerator(KingdomCell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid, float noiseScale, int seed, GameObject[] interestPoints)
            : base(hexPrefab, gridWidth, gridHeight, availableBiomes, parentGrid, noiseScale, seed, interestPoints)
        {
            _perlinTerrainManager = new PerlinTerrainManager(noiseScale, seed);
            _voronoiBiomeManager = new VoronoiBiomeManager(gridWidth, gridHeight, availableBiomes.Length, seed);
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid()
        {
            Dictionary<Vector2Int, Cell> gridCells = new();

            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexSize, x, y);
                    Cell cell = Object.Instantiate(HexPrefab, centerPosition, Quaternion.identity, ParentGrid);
                    cell.name = $"Cell[{x};{y}]";
                    int biomeIndex = _voronoiBiomeManager.GetClosestBiomeIndex(x, y);
                    BiomeTypeSO selectedBiome = AvailableBiomes[biomeIndex];
                    SetupCell(cell, x, y, selectedBiome, HexSize);
                    gridCells[new Vector2Int(x, y)] = cell;
                }
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
            {
                if (perlinValue < (i + 1) * segmentSize)
                {
                    return availableTerrains[i];
                }
            }
            return availableTerrains[terrainCount - 1];
        }

        public void SetupInterestPoints(Dictionary<Vector2Int, Cell> cells)
        {
            Debug.Log("Setup Interest Points...");
            List<KingdomCell> kingdomCells = new List<KingdomCell>();
            foreach (KingdomCell item in cells.Values)
            {
                if (item.IsFree())
                {
                    kingdomCells.Add(item);
                }
            }

            if (kingdomCells.Count < InterestPoints.Length)
            {
                Debug.LogWarning(string.Format("Not enough Free cells for InterestPoints number :\n[KingdomCells = {0}]\n[InterestPoints = {1}]", kingdomCells.Count, InterestPoints.Length));
                return;
            }

            foreach (GameObject point in InterestPoints)
            {
                KingdomCell cell = Randomizer.GetRandomElementFromArray(kingdomCells.ToArray());
                InterestPoint interestPoint = point.GetComponent<InterestPoint>();
                cell.SetOccupier(interestPoint);
                Object.Instantiate(interestPoint.InterestPointConfiguration.InterestPointPrefab, new Vector3(cell.Coordinates.x, cell.Coordinates.y), Quaternion.identity, cell.transform);
                kingdomCells.Remove(cell);
            }

        }
        public override string ToString()
        {
            return $"BaseGridGenerator:\n" +
                    $"- HexPrefab: {HexPrefab.name}\n" +
                    $"- GridWidth: {GridWidth}\n" +
                    $"- GridHeight: {GridHeight}\n" +
                    $"- Available Biomes: {(AvailableBiomes != null && AvailableBiomes.Length > 0 ? string.Join(", ", AvailableBiomes.Select(b => b.name)) : "None")}\n" +
                    $"- ParentGrid: {ParentGrid?.name ?? "None"}\n" +
                    $"- NoiseScale: {(NoiseScale.HasValue ? NoiseScale.Value.ToString() : "None")}\n" +
                    $"- Seed: {Seed?.ToString() ?? "None"}";
        }

    }
}
