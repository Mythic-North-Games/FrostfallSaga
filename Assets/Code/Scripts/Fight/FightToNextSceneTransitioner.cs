using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Fight.UI;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.Fight
{
    public class FightToNextSceneTransitioner : MonoBehaviour
    {
        [SerializeField] private FightEndMenuController _fightEndMenuController;
        [SerializeField] private SceneTransitioner _sceneTransitioner;

        private void OnContinueClicked()
        {
            GameStateManager gameStateManager = GameStateManager.Instance;

            // Get the scene to transition to
            EScenesName sceneToTransitionTo = gameStateManager.GetCurrentFightOrigin().ToEScenesName();

            // Clean pre fight data for next fight
            gameStateManager.CleanPreFightData();

            // Do the transition
            Debug.Log($"Transitioning back to {sceneToTransitionTo}...");
            _sceneTransitioner.FadeInToScene(sceneToTransitionTo.ToSceneString());
        }

        #region Setup & tear down

        private void Awake()
        {
            if (_fightEndMenuController == null)
            {
                _fightEndMenuController = FindObjectOfType<FightEndMenuController>();
            }
            if (_fightEndMenuController == null)
            {
                Debug.LogError("Fight End Menu Controller not found. Can't know when to transition to kingdom.");
            }

            if (_sceneTransitioner == null)
            {
                _sceneTransitioner = FindObjectOfType<SceneTransitioner>();
            }
            if (_sceneTransitioner == null)
            {
                Debug.LogError("Scene transitioner not found. Can't transition to kingdom.");
            }

            _fightEndMenuController.onContinueClicked += OnContinueClicked;
        }

        #endregion
    }
}