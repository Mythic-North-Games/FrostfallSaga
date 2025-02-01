using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.Kingdom
{
    public class CityLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager _kingdomManager;
        [SerializeField] private EnterCityPanelController _enterCityPanelController;

        private void OnCityEnterClicked(CityBuildingConfigurationSO cityBuildingConfiguration)
        {
            Debug.Log($"Saving kingdom state before loading city scene for {cityBuildingConfiguration.Name}.");
            _kingdomManager.SaveKingdomState();

            Debug.Log($"Saving city load data for {cityBuildingConfiguration.Name}.");
            GameStateManager.Instance.SaveCityLoadData(cityBuildingConfiguration.InCityConfiguration);

            Debug.Log($"Launching city scene...");
            SceneTransitioner.Instance.FadeInToScene(EScenesName.CITY.ToSceneString());
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