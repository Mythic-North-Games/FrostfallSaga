using UnityEngine;
using FrostfallSaga.City;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Kingdom.CityBuildings;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.KingdomToCity
{
    public class CityLoader : MonoBehaviour
    {
        [SerializeField] private EntitiesGroupsManager _entitiesGroupsManager;
        [SerializeField] private EnterCityPanelController _enterCityPanelController;
        [SerializeField] private CityBuildingToCityDBSO _cityBuildingToCityDB;
        [SerializeField] private string _citySceneName;

        private CityLoadData _cityLoadData;

        private void OnCityEnterClicked(CityBuilding cityBuilding)
        {
            Debug.Log($"Saving kingdom state before loading city scene for {cityBuilding.CityBuildingConfiguration.Name}.");
            KingdomState.Instance.SaveKingdomData(
                _entitiesGroupsManager.HeroGroup,
                _entitiesGroupsManager.EnemiesGroups,
                _entitiesGroupsManager.CityBuildings
            );

            Debug.Log($"Loading city scene for {cityBuilding.CityBuildingConfiguration.Name} city building.");
            CityConfigurationSO cityConfiguration = _cityBuildingToCityDB.GetCityConfigurationByCityBuildingConfiguration(cityBuilding.CityBuildingConfiguration);
            if (cityConfiguration == null)
            {
                Debug.LogError($"No city mapped to {cityBuilding.CityBuildingConfiguration.Name} city building. Can't load city scene.");
                return;
            }

            _cityLoadData.cityConfiguration = cityConfiguration;
            Debug.Log($"City scene loaded for {cityBuilding.CityBuildingConfiguration.Name} city building.");
            Debug.Log($"Launching city scene...");
            SceneTransitioner.Instance.FadeInToScene(_citySceneName);
        }


        #region Setup
        private void Awake()
        {
            if (_enterCityPanelController == null)
            {
                Debug.LogError("No EntitiesGroupsManager assigned to CityLoader. Won't be able to load city scene correctly.");
                return;
            }

            if (_cityBuildingToCityDB == null)
            {
                Debug.LogError("No CityBuildingToCityDBSO assigned to CityLoader. Won't be able to load city scene correctly.");
                return;
            }

            if (_entitiesGroupsManager == null)
            {
                _entitiesGroupsManager = FindObjectOfType<EntitiesGroupsManager>();
            }
            if (_entitiesGroupsManager == null)
            {
                Debug.LogError("No EntitiesGroupsManager found in scene. Won't be able to save kingdom state.");
                return;
            }

            _enterCityPanelController.onCityEnterClicked += OnCityEnterClicked;
            _cityLoadData = CityLoadData.Instance;
        }
        #endregion
    }
}