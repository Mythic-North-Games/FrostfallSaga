using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;

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
    }
}