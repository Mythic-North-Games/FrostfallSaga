using System.Linq;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class PostFightDataGenerator : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;

        #region Setup & tear down

        private void Awake()
        {
            if (_fightManager == null) _fightManager = FindObjectOfType<FightManager>();
            if (_fightManager == null)
                Debug.LogError("Fight manager not found. Can't know when to transition to kingdom.");
            _fightManager.onFightEnded += OnFightEnded;
        }

        #endregion

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            GameStateManager gameStateManager = GameStateManager.Instance;

            if (gameStateManager.GetCurrentFightOrigin() == EFightOrigin.DUNGEON)
            {
                Debug.Log("Saving dungeon progress...");

                var alliesHaveWon = allies.All(ally => ally.GetHealth() > 0);
                gameStateManager.SaveDungeonProgress(alliesHaveWon);

                Debug.Log("Dungeon progress saved!");
            }

            if (enemies[0].EntitySessionId == null || enemies[0].EntitySessionId.Length == 0)
            {
                Debug.Log("No session ID on fighters. No post fight data saved.");
                gameStateManager.CleanPostFightData();
                return;
            }

            gameStateManager.SavePostFightData(enemies);
            Debug.Log("Post fight data saved!");
        }
    }
}