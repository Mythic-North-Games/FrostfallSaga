using System;

namespace FrostfallSaga.Grid
{
    public static class GridFactory
    {
        public static IGridGenerator CreateGridGenerator(EGridType gridType, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, float? noiseScale = null, int? seed = null)
        {
            switch (gridType)
            {
                case EGridType.KINGDOM:
                    if (noiseScale.HasValue && seed.HasValue)
                    {
                        return new KingdomGridGenerator(gridWidth, gridHeight, availableBiomes, noiseScale.Value, seed.Value);
                    }
                    else
                    {
                        throw new ArgumentException("KingdomGridGenerator requires noiseScale and seed.");
                    }
                case EGridType.FIGHT:
                    if (noiseScale.HasValue && seed.HasValue)
                    {
                        return new FightGridGenerator(gridWidth, gridHeight, availableBiomes, noiseScale.Value, seed.Value);
                    }
                    else
                    {
                        throw new ArgumentException("FightGridGenerator requires noiseScale and seed.");
                    }
                case EGridType.DUNGEON:
                    return new DungeonGridGenerator(gridWidth, gridHeight, availableBiomes);
                default:
                    throw new ArgumentException("Invalid grid type.");
            }
        }
    }
}
