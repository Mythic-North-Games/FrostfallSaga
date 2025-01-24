using System;
using FrostfallSaga.City;
using FrostfallSaga.Kingdom.CityBuildings;

namespace FrostfallSaga.KingdomToCity
{
    [Serializable]
    public class CityBuildingToCity
    {
        public CityBuildingConfigurationSO cityBuildingConfiguration;
        public CityConfigurationSO cityConfiguration;
    }
}