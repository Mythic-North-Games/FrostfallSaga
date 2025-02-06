using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Kingdom.UI;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.Kingdom
{
    public class DungeonLoader : MonoBehaviour
    {
        [SerializeField] private KingdomManager _kingdomManager;
        [SerializeField] private EnterDungeonPanelController _enterDungeonPanelController;

        private void OnDungeonEnterClicked(DungeonBuildingConfigurationSO dungeonBuildingConfiguration)
        {
            Debug.Log($"Saving kingdom state before loading dungeon scene for {dungeonBuildingConfiguration.Name}.");
            _kingdomManager.SaveKingdomState();

            Debug.Log($"Saving dungeon load data for {dungeonBuildingConfiguration.Name}.");
            GameStateManager.Instance.InitDungeonState(dungeonBuildingConfiguration.DungeonConfiguration);

            Debug.Log($"Launching dungeon scene...");
            SceneTransitioner.Instance.FadeInToScene(EScenesName.DUNGEON.ToSceneString());
        }


        #region Setup
        private void Awake()
        {
            if (_enterDungeonPanelController == null)
            {
                Debug.LogError("No EnterDungeonPanelController assigned to DungeonLoader. Won't be able to load dungeon scene correctly.");
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

            _enterDungeonPanelController.onDungeonEnterClicked += OnDungeonEnterClicked;
        }
        #endregion
    }
}