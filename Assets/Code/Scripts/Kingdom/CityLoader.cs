using System.Collections;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class CityLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager _kingdomManager;
        [SerializeField] private BaseEnterInterestPointPanelUIController _enterCityPanelController;
        [SerializeField] private float _delayBeforeLoadingCityScene = 0.6f;

        #region Setup

        private void Awake()
        {
            if (_enterCityPanelController == null)
            {
                Debug.LogError(
                    "No enter EnterCityPanelController assigned to CityLoader. Won't be able to load city scene correctly.");
                return;
            }

            if (_kingdomManager == null) _kingdomManager = FindObjectOfType<KingdomManager>();
            if (_kingdomManager == null)
            {
                Debug.LogError("No KingdomManager found in scene. Won't be able to save kingdom state.");
                return;
            }

            _enterCityPanelController.onInterestPointEnterClicked += OnCityEnterClicked;
        }

        #endregion

        private void OnCityEnterClicked(AInterestPointConfigurationSO interestPointConfiguration)
        {
            if (interestPointConfiguration is not CityBuildingConfigurationSO cityBuildingConfig)
            {
                Debug.LogError("CityBuildingConfigurationSO is null. Cannot load city scene.");
                return;
            }

            Debug.Log($"Saving kingdom state before loading city scene for {cityBuildingConfig.Name}.");
            _kingdomManager.SaveKingdomState();

            Debug.Log($"Saving city load data for {cityBuildingConfig.Name}.");
            GameStateManager.Instance.SaveCityLoadData(cityBuildingConfig.InCityConfiguration);

            Debug.Log("Launching city scene...");
            StartCoroutine(WaitAndLaunchCityScene());
        }

        private IEnumerator WaitAndLaunchCityScene()
        {
            yield return new WaitForSeconds(_delayBeforeLoadingCityScene);
            SceneTransitioner.TransitionToScene(EScenesName.CITY.ToSceneString());
        }
    }
}