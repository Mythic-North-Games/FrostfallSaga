using System;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    /// <summary>
    /// Responsible for controlling the fight end menu.
    /// </summary>
    public class FightEndMenuController : BaseUIController
    {
        private static readonly string RESULT_LABEL_UI_NAME = "FightResultLabel";
        private static readonly string ROUND_COUNT_LAEBL_UI_NAME = "RoundCountLabel";

        public Action onContinueClicked;

        [SerializeField] private FightManager _fightManager;

        private void Start()
        {
            _uiDoc.rootVisualElement.style.visibility = Visibility.Hidden;
        }

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            _uiDoc.rootVisualElement.style.visibility = Visibility.Visible;
            _uiDoc.rootVisualElement.Q<Label>(RESULT_LABEL_UI_NAME).text = AlliesHaveWon(allies) ? "Winner" : "Looser";
            _uiDoc.rootVisualElement.Q<Label>(ROUND_COUNT_LAEBL_UI_NAME).text = $"Rounds: {_fightManager.RoundCount}";
        }

        private bool AlliesHaveWon(Fighter[] allies)
        {
            foreach (Fighter ally in allies)
            {
                if (ally.GetHealth() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnContinueButtonClicked(ClickEvent clickEvent)
        {
            _uiDoc.rootVisualElement.style.display = DisplayStyle.None;
            onContinueClicked?.Invoke();
        }

        #region Setup & teardown

        private void Awake()
        {

            if (_uiDoc == null)
            {
                _uiDoc = GetComponent<UIDocument>();
            }
            if (_uiDoc == null)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            if (_fightManager == null)
            {
                _fightManager = FindObjectOfType<FightManager>();
            }
            if (_fightManager == null)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            _fightManager.onFightEnded += OnFightEnded;

            Button continueButton = _uiDoc.rootVisualElement.Q("EndFightNextButton") as Button;
            continueButton.RegisterCallback<ClickEvent>(OnContinueButtonClicked);

        }

        private void Disable()
        {
            if (_fightManager != null)
            {
                _fightManager.onFightEnded += OnFightEnded;
            }
        }

        #endregion
    }
}