using System;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.HeroTeam;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class TavernDialogController
    {
        private static readonly string WELCOME_LABEL_UI_NAME = "WelcomeLabel";
        private static readonly string REST_QUESTION_UI_NAME = "QuestionLabel";
        private static readonly string REST_BUTTON_UI_NAME = "RestButton";
        private static readonly string EXIT_BUTTON_UI_NAME = "ExitButton";
        private static readonly string TAVERN_DIALOG_HIDDEN_CLASSNAME = "tavernDialogHidden";

        private readonly VisualElement _dialogRoot;
        public Action onExitClicked;

        public Action onRestButtonClicked;

        public TavernDialogController(VisualElement tavernDialogRoot)
        {
            _dialogRoot = tavernDialogRoot;
        }

        public void SetupTavernDialog(TavernConfiguration tavernConfiguration)
        {
            _dialogRoot.Q<Label>(WELCOME_LABEL_UI_NAME).text = $"Welcome to {tavernConfiguration.Name}!";
            _dialogRoot.Q<Label>(REST_QUESTION_UI_NAME).text =
                $"Would you like to rest for {tavernConfiguration.RestCost} stycas?";
            _dialogRoot.Q<Button>(REST_BUTTON_UI_NAME).clicked += OnRestButtonClicked;
            _dialogRoot.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;
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
            HeroTeam.Instance.FullHealTeam();
            Debug.Log("Team fully healed.");
            onRestButtonClicked?.Invoke();
        }

        public void OnExitButtonClicked()
        {
            onExitClicked?.Invoke();
        }
    }
}