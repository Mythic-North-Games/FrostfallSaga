using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using FrostfallSaga.Core.GameState.Kingdom;

namespace FrostfallSaga.Kingdom.CityBuildings
{
    public class CityBuildingBuilder : MonoBehaviourPersistingSingleton<CityBuildingBuilder>
    {
        public CityBuilding BuildCityBuilding(CityBuildingData cityBuildingData, HexGrid grid)
        {
            GameObject cityBuildingPrefab = WorldGameObjectInstantiator.Instance.Instantiate(
                cityBuildingData.cityBuildingConfiguration.CityPrefab
            );
            CityBuilding cityBuilding = cityBuildingPrefab.GetComponent<CityBuilding>();
            cityBuilding.cell = grid.CellsByCoordinates[new(cityBuildingData.cellX, cityBuildingData.cellY)] as KingdomCell;
            cityBuilding.transform.position = cityBuilding.cell.GetCenter();
            return cityBuilding;
        }

        public CityBuildingData ExtractCityBuildingDataFromBuilding(CityBuilding cityBuilding)
        {
            return new(
                cityBuilding.CityBuildingConfiguration,
                cityBuilding.cell.Coordinates.x,
                cityBuilding.cell.Coordinates.y
            );
        }
    }
}