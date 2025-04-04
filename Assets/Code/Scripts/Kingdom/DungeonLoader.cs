using FrostfallSaga.Audio;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class DungeonLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager kingdomManager;
        [SerializeField] private EnterDungeonPanelController enterDungeonPanelController;


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
            enterDungeonPanelController.onDungeonEnterClicked += OnDungeonEnterClicked;
        }

        #endregion

        private void OnDungeonEnterClicked(DungeonBuildingConfigurationSO dungeonBuildingConfiguration)
        {
            Debug.Log($"Saving kingdom state before loading dungeon scene for {dungeonBuildingConfiguration.Name}.");
            kingdomManager.SaveKingdomState();
            
            Debug.Log($"Saving dungeon load data for {dungeonBuildingConfiguration.Name}.");
            GameStateManager.Instance.InitDungeonState(dungeonBuildingConfiguration.DungeonConfiguration);
            
            AudioManager.Instance.PlayUISound(dungeonBuildingConfiguration.EnterDungeonSound);

            Debug.Log("Launching dungeon scene...");
            SceneTransitioner.FadeInToScene(EScenesName.DUNGEON.ToSceneString());
        }
    }
}