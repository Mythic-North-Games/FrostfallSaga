using FrostfallSaga.Core;
using FrostfallSaga.Fight.UI;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightToKingdomTransitioner : MonoBehaviour
    {
        [SerializeField] private FightEndMenuController _fightEndMenuController;
        [SerializeField] private SceneTransitioner _sceneTransitioner;
        [SerializeField] private string _kingdomSceneName;

        private void OnContinueClicked()
        {
            Debug.Log("Transitioning to kingdom...");
            _sceneTransitioner.FadeInToScene(_kingdomSceneName);
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

        private void OnDisable()
        {
            if (_fightEndMenuController != null)
            {
                _fightEndMenuController.onContinueClicked -= OnContinueClicked;
            }
        }

        #endregion
    }
}