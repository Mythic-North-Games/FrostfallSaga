using System.Linq;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;

namespace FrostfallSaga.Fight
{
    public class PostFightDataGenerator : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            GameStateManager gameStateManager = GameStateManager.Instance;

            if (gameStateManager.GetCurrentFightOrigin() == EFightOrigin.DUNGEON)
            {
                Debug.Log("Saving dungeon progress...");

                bool alliesHaveWon = allies.All(ally => ally.GetHealth() > 0);
                gameStateManager.SaveDungeonProgress(alliesHaveWon);

                Debug.Log("Dungeon progress saved!");
            }

            if (allies[0].EntitySessionId == null || allies[0].EntitySessionId.Length == 0)
            {
                Debug.Log("No session ID on fighters. No post fight data saved.");
                gameStateManager.CleanPostFightData();
                return;
            }

            gameStateManager.SavePostFightData(allies, enemies);
            Debug.Log("Post fight data saved!");
        }

        #region Setup & tear down

        private void Awake()
        {
            if (_fightManager == null)
            {
                _fightManager = FindObjectOfType<FightManager>();
            }
            if (_fightManager == null)
            {
                Debug.LogError("Fight manager not found. Can't know when to transition to kingdom.");
            }
            _fightManager.onFightEnded += OnFightEnded;
        }

        #endregion
    }
}