using System;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public static class GridFactory
    {
        public static IGridGenerator CreateGridGenerator(EGridType gridType, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid, float? noiseScale = null, int? seed = null)
        {
            bool needsNoise = gridType == EGridType.KINGDOM || gridType == EGridType.FIGHT;
            if (needsNoise && (!noiseScale.HasValue || !seed.HasValue))
            {
                throw new ArgumentException($"{gridType} requires noiseScale and seed.");
            }

            return gridType switch
            {
                EGridType.KINGDOM => new KingdomGridGenerator(gridWidth, gridHeight, availableBiomes, parentGrid, noiseScale.Value, seed.Value),
                EGridType.FIGHT => new FightGridGenerator(gridWidth, gridHeight, availableBiomes, parentGrid, noiseScale.Value, seed.Value),
                EGridType.DUNGEON => new DungeonGridGenerator(gridWidth, gridHeight, availableBiomes, parentGrid),
                _ => throw new ArgumentException($"{gridType} : Invalid grid type.")
            };
        }
    }
}
