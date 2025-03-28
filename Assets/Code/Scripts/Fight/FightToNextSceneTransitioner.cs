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
            fightEndMenuController ??= FindObjectOfType<FightEndMenuController>();
            if (!fightEndMenuController)
                Debug.LogError("Fight End Menu Controller not found. Can't know when to transition to kingdom.");

            sceneTransitioner ??= FindObjectOfType<SceneTransitioner>();
            if (!sceneTransitioner) Debug.LogError("Scene transitioner not found. Can't transition to kingdom.");

            fightEndMenuController.onContinueClicked += OnContinueClicked;
        }

        #endregion

        private static void OnContinueClicked()
        {
            EScenesName sceneToTransitionTo = GameStateManager.Instance.GetCurrentFightOrigin().ToEScenesName();
            GameStateManager.Instance.CleanPreFightData();
            Debug.Log($"Transitioning back to {sceneToTransitionTo}...");
            SceneTransitioner.FadeInToScene(sceneToTransitionTo.ToSceneString());
        }
    }
}