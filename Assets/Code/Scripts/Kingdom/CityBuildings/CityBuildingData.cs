using System;

namespace FrostfallSaga.Kingdom.CityBuildings
{
    [Serializable]
    public class CityBuildingData
    {
        public CityBuildingConfigurationSO cityBuildingConfiguration;
        public int cellX;
        public int cellY;

        public CityBuildingData(CityBuilding cityBuilding)
        {
            cityBuildingConfiguration = cityBuilding.CityBuildingConfiguration;
            cellX = cityBuilding.cell.Coordinates.x;
            cellY = cityBuilding.cell.Coordinates.y;
        }
    }
}
