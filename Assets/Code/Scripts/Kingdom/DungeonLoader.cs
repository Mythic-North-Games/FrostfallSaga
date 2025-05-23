using System.Collections;
using FrostfallSaga.Audio;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class DungeonLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager kingdomManager;
        [SerializeField] private BaseEnterInterestPointPanelUIController enterDungeonPanelController;
        [SerializeField] private float _delayBeforeLoadingCityScene = 0.6f;

        #region Setup

        private void Awake()
        {
            if (!enterDungeonPanelController)
            {
                Debug.LogError(
                    "No EnterDungeonPanelController assigned to DungeonLoader. Won't be able to load dungeon scene correctly.");
                return;
            }

            kingdomManager ??= FindObjectOfType<KingdomManager>();
            if (!kingdomManager)
            {
                Debug.LogError("No KingdomManager found in scene. Won't be able to save kingdom state.");
                return;
            }

            enterDungeonPanelController.onInterestPointEnterClicked += OnDungeonEnterClicked;
        }

        #endregion

        private void OnDungeonEnterClicked(AInterestPointConfigurationSO interestPointConfiguration)
        {
            if (interestPointConfiguration is not DungeonBuildingConfigurationSO dungeonConfiguration)
            {
                Debug.LogError("DungeonBuildingConfigurationSO is null. Cannot load dungeon scene.");
                return;
            }

            Debug.Log($"Saving kingdom state before loading dungeon scene for {dungeonConfiguration.Name}.");
            kingdomManager.SaveKingdomState();

            Debug.Log($"Saving dungeon load data for {dungeonConfiguration.Name}.");
            GameStateManager.Instance.InitDungeonState(dungeonConfiguration.DungeonConfiguration);

            AudioManager.Instance.PlayUISound(dungeonConfiguration.EnterDungeonSound);

            Debug.Log("Launching dungeon scene...");
            StartCoroutine(WaitAndLaunchDungeonScene());
        }

        private IEnumerator WaitAndLaunchDungeonScene()
        {
            yield return new WaitForSeconds(_delayBeforeLoadingCityScene);
            SceneTransitioner.TransitionToScene(EScenesName.DUNGEON.ToSceneString());
        }
    }
}