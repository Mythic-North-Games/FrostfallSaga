using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Core.GameState;

namespace FrostfallSaga.Fight
{
    public class PostFightDataGenerator : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            if (allies[0].EntitySessionId == null || allies[0].EntitySessionId.Length == 0)
            {
                Debug.Log("No session ID on fighters. Dev mod assumed. No post fight data saved.");
                GameStateManager.Instance.CleanPostFightData();
                return;
            }
            GameStateManager.Instance.SavePostFightData(allies, enemies);
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