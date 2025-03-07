using System;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.HeroTeam;

namespace FrostfallSaga.City.UI
{
    public class TavernDialogController
    {
        private readonly static string WELCOME_LABEL_UI_NAME = "WelcomeLabel";
        private readonly static string REST_QUESTION_UI_NAME = "QuestionLabel";
        private readonly static string REST_BUTTON_UI_NAME = "RestButton";
        private readonly static string EXIT_BUTTON_UI_NAME = "ExitButton";
        private readonly static string TAVERN_DIALOG_HIDDEN_CLASSNAME = "tavernDialogHidden";

        public Action onRestButtonClicked;
        public Action onExitClicked;

        private VisualElement _dialogRoot;
        private TavernConfiguration _tavernConfiguration;

        public TavernDialogController(VisualElement tavernDialogRoot)
        {
            _dialogRoot = tavernDialogRoot;
        }

        public void SetupTavernDialog(TavernConfiguration tavernConfiguration)
        {
            _tavernConfiguration = tavernConfiguration;
            _dialogRoot.Q<Label>(WELCOME_LABEL_UI_NAME).text = $"Welcome to {tavernConfiguration.Name}!";
            _dialogRoot.Q<Label>(REST_QUESTION_UI_NAME).text = $"Would you like to rest for {tavernConfiguration.RestCost} stycas?";
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

        public void OnRestButtonClicked()
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
            onRestButtonClicked?.Invoke();
        }

        public void OnExitButtonClicked()
        {
            onExitClicked?.Invoke();
        }
    }
}