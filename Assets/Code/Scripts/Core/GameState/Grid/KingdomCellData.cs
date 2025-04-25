using System;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Core.GameState.Grid
{
    [Serializable]
    public class KingdomCellData : CellData
    {
        private static readonly string PrefabPath = "Prefabs/Grid/KingdomCell";

        public KingdomCellData(
            int cellX,
            int cellY,
            TerrainTypeSO terrainType,
            BiomeTypeSO biomeType,
            ECellHeight height,
            bool isAccessible,
            CellVisualData cellVisualData = null
        ) : base(PrefabPath, cellX, cellY, terrainType, biomeType, height, isAccessible, cellVisualData)
        {
        }
    }
}