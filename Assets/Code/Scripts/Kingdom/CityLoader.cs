using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Kingdom.CityBuildings;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.Kingdom
{
    public class CityLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager _kingdomManager;
        [SerializeField] private EnterCityPanelController _enterCityPanelController;

        private void OnCityEnterClicked(CityBuilding cityBuilding)
        {
            Debug.Log($"Saving kingdom state before loading city scene for {cityBuilding.CityBuildingConfiguration.Name}.");
            _kingdomManager.SaveKingdomState();

            Debug.Log($"Saving city load data for {cityBuilding.CityBuildingConfiguration.Name}.");
            GameStateManager.Instance.SaveCityLoadData(cityBuilding.CityBuildingConfiguration.InCityConfiguration);

            Debug.Log($"Launching city scene...");
            SceneTransitioner.Instance.FadeInToScene(EScenesName.City.ToString());
        }


        #region Setup
        private void Awake()
        {
            if (_enterCityPanelController == null)
            {
                Debug.LogError("No KingdomManager assigned to CityLoader. Won't be able to load city scene correctly.");
                return;
            }

            if (_kingdomManager == null)
            {
                _kingdomManager = FindObjectOfType<KingdomManager>();
            }
            if (_kingdomManager == null)
            {
                Debug.LogError("No KingdomManager found in scene. Won't be able to save kingdom state.");
                return;
            }

            _enterCityPanelController.onCityEnterClicked += OnCityEnterClicked;
        }
        #endregion
    }
}