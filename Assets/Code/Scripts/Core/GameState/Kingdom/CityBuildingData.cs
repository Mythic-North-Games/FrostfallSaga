using System;
using FrostfallSaga.Core.Cities;

namespace FrostfallSaga.Core.GameState.Kingdom
{
    [Serializable]
    public class CityBuildingData
    {
        public CityBuildingConfigurationSO cityBuildingConfiguration;
        public int cellX;
        public int cellY;

        public CityBuildingData(CityBuildingConfigurationSO config, int cellX, int cellY)
        {
            cityBuildingConfiguration = config;
            this.cellX = cellX;
            this.cellY = cellY;
        }
    }
}
