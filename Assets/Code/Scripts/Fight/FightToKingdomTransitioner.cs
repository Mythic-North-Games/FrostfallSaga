using System.Linq;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight
{
    public class FightToKingdomTransitioner : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;
        [SerializeField] private PostFightDataSO _postFightData;
        [SerializeField] private SceneTransitioner _sceneTransitioner;
        [SerializeField] private string _kingdomSceneName;

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            SavePostFightData(allies, enemies);
            _sceneTransitioner.FadeInToScene(_kingdomSceneName);
        }

        private void SavePostFightData(Fighter[] allies, Fighter[] enemies)
        {
            _postFightData.alliesState = new();
            allies.ToList().ForEach(ally => _postFightData.alliesState.Add(ally.EntitySessionId, new(ally)));

            _postFightData.enemiesState = new();
            enemies.ToList().ForEach(enemy => _postFightData.alliesState.Add(enemy.EntitySessionId, new(enemy)));

            Debug.Log("Post fight data saved!");
        }

        #region Setup & teardown

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
                Debug.LogError("Post fight data not found. Can't save the need post fight data.");
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