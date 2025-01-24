using System.Linq;
using UnityEngine;
using FrostfallSaga.City;
using FrostfallSaga.Kingdom.CityBuildings;

namespace FrostfallSaga.KingdomToCity
{
    [CreateAssetMenu(fileName = "CityBuildingToCityDBSO", menuName = "ScriptableObjects/CityBuildingToCityDBSO", order = 0)]
    public class CityBuildingToCityDBSO : ScriptableObject
    {
        [field: SerializeField] public CityBuildingToCity[] DB { get; private set; }

        /// <summary>
        /// Returns the city configuration for the given city building configuration.
        /// </summary>
        /// <param name="cityBuildingConfiguration">The city building configuration to get the city configuration for.</param>
        /// <returns>The city configuration for the given city building configuration or null if not found.</returns>
        public CityConfigurationSO GetCityConfigurationByCityBuildingConfiguration(CityBuildingConfigurationSO cityBuildingConfiguration)
        {
            if (
                DB.First(
                    mapping => mapping.cityBuildingConfiguration == cityBuildingConfiguration
                ) is CityBuildingToCity cityBuildingToCity
            )
            {
                return cityBuildingToCity.cityConfiguration;
            }
            else
            {
                return null;
            }
        }
    }
}