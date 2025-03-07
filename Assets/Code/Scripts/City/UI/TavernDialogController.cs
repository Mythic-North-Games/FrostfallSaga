using System;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.HeroTeam;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class TavernDialogController
    {
        #region UXML Names and classes
        private static readonly string WELCOME_LABEL_UI_NAME = "WelcomeLabel";
        private static readonly string REST_QUESTION_UI_NAME = "QuestionLabel";
        private static readonly string REST_BUTTON_UI_NAME = "RestButton";
        private static readonly string EXIT_BUTTON_UI_NAME = "ExitButton";
        private static readonly string TAVERN_DIALOG_HIDDEN_CLASSNAME = "tavernDialogHidden";
        #endregion

        public Action OnExitClicked;
        public Action OnRestButtonClicked;

        private readonly VisualElement _dialogRoot;
        private TavernConfiguration _tavernConfiguration;

        public TavernDialogController(VisualElement tavernDialogRoot)
        {
            _dialogRoot = tavernDialogRoot;
        }

        public void SetupTavernDialog(TavernConfiguration tavernConfiguration)
        {
            _tavernConfiguration = tavernConfiguration;
            _dialogRoot.Q<Label>(WELCOME_LABEL_UI_NAME).text = $"Welcome to {tavernConfiguration.Name}!";
            _dialogRoot.Q<Label>(REST_QUESTION_UI_NAME).text =
                $"Would you like to rest for {tavernConfiguration.RestCost} stycas?";
            _dialogRoot.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;

            if (HeroTeam.Instance.Stycas < tavernConfiguration.RestCost)
            {
                _dialogRoot.Q<Button>(REST_BUTTON_UI_NAME).SetEnabled(false);
                return;
            }
            _dialogRoot.Q<Button>(REST_BUTTON_UI_NAME).SetEnabled(true);
            _dialogRoot.Q<Button>(REST_BUTTON_UI_NAME).clicked += OnRestButtonClicked;
        }

        public void Display()
        {
            _dialogRoot.RemoveFromClassList(TAVERN_DIALOG_HIDDEN_CLASSNAME);
        }

        public void Hide()
        {
            _dialogRoot.AddToClassList(TAVERN_DIALOG_HIDDEN_CLASSNAME);
        }

        public void RestButtonClicked()
        {
            HeroTeam heroTeam = HeroTeam.Instance;

            // Check if the team has enough stycas to rest
            if (heroTeam.Stycas < _tavernConfiguration.RestCost)
            {
                Debug.Log("Not enough stycas to rest.");
                return;
            }

            // If so, withdraw the stycas and fully heal the team
            heroTeam.WithdrawStycas(_tavernConfiguration.RestCost);
            heroTeam.FullHealTeam();

            Debug.Log("Team fully healed.");
            OnRestButtonClicked?.Invoke();
        }

        public void OnExitButtonClicked()
        {
            OnExitClicked?.Invoke();
        }
    }
}