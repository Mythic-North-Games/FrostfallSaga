using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Fight.UI;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightToNextSceneTransitioner : MonoBehaviour
    {
        [SerializeField] private FightEndMenuController fightEndMenuController;
        [SerializeField] private SceneTransitioner sceneTransitioner;

        #region Setup & tear down

        private void Awake()
        {
            if (fightEndMenuController == null) fightEndMenuController = FindObjectOfType<FightEndMenuController>();
            if (fightEndMenuController == null)
                Debug.LogError("Fight End Menu Controller not found. Can't know when to transition to kingdom.");

            if (sceneTransitioner == null) sceneTransitioner = FindObjectOfType<SceneTransitioner>();
            if (sceneTransitioner == null)
                Debug.LogError("Scene transitioner not found. Can't transition to kingdom.");

            fightEndMenuController.onContinueClicked += OnContinueClicked;
        }

        #endregion

        private void OnContinueClicked()
        {
            GameStateManager gameStateManager = GameStateManager.Instance;

            // Get the scene to transition to
            EScenesName sceneToTransitionTo = gameStateManager.GetCurrentFightOrigin().ToEScenesName();

            // Clean pre fight data for next fight
            gameStateManager.CleanPreFightData();

            // Do the transition
            Debug.Log($"Transitioning back to {sceneToTransitionTo}...");
            sceneTransitioner.FadeInToScene(sceneToTransitionTo.ToSceneString());
        }
    }
}