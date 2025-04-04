using FrostfallSaga.Core;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class CityLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager kingdomManager;
        [SerializeField] private EnterCityPanelController enterCityPanelController;


        #region Setup

        private void Awake()
        {
            if (enterCityPanelController == null)
            {
                Debug.LogError(
                    "No enter EnterCityPanelController assigned to CityLoader. Won't be able to load city scene correctly.");
                return;
            }

            if (kingdomManager == null) kingdomManager = FindObjectOfType<KingdomManager>();
            if (kingdomManager == null)
            {
                Debug.LogError("No KingdomManager found in scene. Won't be able to save kingdom state.");
                return;
            }

            enterCityPanelController.onCityEnterClicked += OnCityEnterClicked;
        }

        #endregion

        private void OnCityEnterClicked(CityBuildingConfigurationSO cityBuildingConfiguration)
        {
            Debug.Log($"Saving kingdom state before loading city scene for {cityBuildingConfiguration.Name}.");
            kingdomManager.SaveKingdomState();

            Debug.Log($"Saving city load data for {cityBuildingConfiguration.Name}.");
            GameStateManager.Instance.SaveCityLoadData(cityBuildingConfiguration.InCityConfiguration);

            Debug.Log("Launching city scene...");
            SceneTransitioner.FadeInToScene(EScenesName.CITY.ToSceneString());
        }
    }
}