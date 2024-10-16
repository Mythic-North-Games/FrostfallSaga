using System;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.UI
{
    /// <summary>
    /// Responsible for controlling the fight end menu.
    /// </summary>
    public class FightEndMenuController : BaseUIController
    {
        public Action onContinueClicked;

        [SerializeField] private FightManager _fightManager;

        private void Start()
        {
            _uiDoc.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            _uiDoc.rootVisualElement.style.display = DisplayStyle.Flex;
            Label textWin = _uiDoc.rootVisualElement.Q("FightResultText") as Label;
            textWin.text = AlliesHaveWon(allies) ? "Winner" : "Looser";
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