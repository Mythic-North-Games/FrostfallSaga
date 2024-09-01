using System.Linq;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight;
using FrostfallSaga.Kingdom;

namespace FrostfallSaga.KingdomToFight
{
    public class FightToKingdomTransitioner : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;
        [SerializeField] private SceneTransitioner _sceneTransitioner;
        [SerializeField] private PostFightDataSO _postFightData;
        [SerializeField] private string _kingdomSceneName;

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            SavePostFightData(allies, enemies);
            Debug.Log("Post fight data saved!");
            Debug.Log("Transitioning to kingdom...");
            _sceneTransitioner.FadeInToScene(_kingdomSceneName);
        }

        private void SavePostFightData(Fighter[] allies, Fighter[] enemies)
        {
            _postFightData.alliesState = new();
            allies.ToList().ForEach(ally => _postFightData.alliesState.Add(ally.EntitySessionId, new(ally.GetHealth())));

            _postFightData.enemiesState = new();
            enemies.ToList().ForEach(enemy => _postFightData.enemiesState.Add(enemy.EntitySessionId, new(enemy.GetHealth())));

            _postFightData.enabled = true;
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

            if (_sceneTransitioner == null)
            {
                _sceneTransitioner = FindObjectOfType<SceneTransitioner>();
            }
            if (_sceneTransitioner == null)
            {
                Debug.LogError("Scene transitioner not found. Can't transition to kingdom.");
            }

            if (_postFightData == null)
            {
                Debug.LogError("No fight data given. Can't properly transition to kingdom scene.");
            }

            _fightManager.onFightEnded += OnFightEnded;
        }

        private void OnDisable()
        {
            if (_fightManager != null)
            {
                _fightManager.onFightEnded -= OnFightEnded;
            }
        }

        #endregion
    }
}