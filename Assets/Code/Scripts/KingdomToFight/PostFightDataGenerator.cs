using System.Linq;
using FrostfallSaga.Fight;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Kingdom;
using UnityEngine;

namespace FrostfallSaga.KingdomToFight
{
    public class PostFightDataGenerator : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;
        
        private PostFightData _postFightData;

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            if (allies[0].EntitySessionId == null || allies[0].EntitySessionId.Length == 0)
            {
                Debug.Log("No session ID on fighters. Dev mod assumed. No post fight data saved.");
                _postFightData.isActive = false;
                return;
            }
            SavePostFightData(allies, enemies);
            Debug.Log("Post fight data saved!");
        }

        private void SavePostFightData(Fighter[] allies, Fighter[] enemies)
        {
            _postFightData.alliesState = new();
            allies.ToList().ForEach(ally => _postFightData.alliesState.Add(new(ally.EntitySessionId, new(ally.GetHealth()))));

            _postFightData.enemiesState = new();
            enemies.ToList().ForEach(enemy => _postFightData.enemiesState.Add(new(enemy.EntitySessionId, new(enemy.GetHealth()))));

            _postFightData.isActive = true;
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
            
            _postFightData = PostFightData.Instance;
        }

        #endregion
    }
}