using System;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Core.GameState.Grid
{
    [Serializable]
    public abstract class CellData
    {
        public string prefabPath;
        public int cellX;
        public int cellY;
        public TerrainTypeSO terrainType;
        public BiomeTypeSO biomeType;
        public ECellHeight height;
        public bool isAccessible;
        public CellVisualData cellVisualData = null;

        protected CellData(
            string prefabPath,
            int cellX,
            int cellY,
            TerrainTypeSO terrainType,
            BiomeTypeSO biomeType,
            ECellHeight height,
            bool isAccessible,
            CellVisualData cellVisualData = null
        )
        {
            this.prefabPath = prefabPath;
            this.cellX = cellX;
            this.cellY = cellY;
            this.terrainType = terrainType;
            this.biomeType = biomeType;
            this.height = height;
            this.isAccessible = isAccessible;
            this.cellVisualData = cellVisualData;
        }
    }
}