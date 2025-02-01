using UnityEngine;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.GameState;

namespace FrostfallSaga.Dungeon
{
    public class DungeonManager : MonoBehaviour
    {
        private void Awake()
        {
            DungeonConfigurationSO dungeonConfiguration = GameStateManager.Instance.GetDungeonConfigurationToLoad();
            Debug.Log($"Ready to manage {dungeonConfiguration.name}");
        }
    }
}