using System;
using System.Collections;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils.Trees;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.Dialogues
{
    public class DialogueUIProcessor : BaseUIController
    {
        [SerializeField] private VisualTreeAsset _answerContainerTemplate;
        [SerializeField] private float _timeBeforeFirstLineDisplay = 1f;
        [SerializeField] private float _timeBetweenDialogueLines = 0.5f;
        [SerializeField] private float _timeBeforeAnswersDisplay = 0.5f;
        [SerializeField] private float _timeBetweenAnswersDisplay = 0.2f;
        private VisualElement _answersContainer;
        private VisualElement _characterIcon;
        private Label _characterNameLabel;

        private DialogueSO _currentDialogue;
        private TreeNode<DialogueLine> _currentDialogueLine;
        private VisualElement _dialogueBoxContainer;
        private VisualElement _dialogueLineContainer;
        private TreeNode<DialogueLine> _previousDialogueLine;
        private VisualElement _richTextContainer;
        private Label _richTextLabel;

        private VisualElement _rootContainer;

        public Action<DialogueSO> onDialogueEnded;

        #region Setup

        private void Awake()
        {
            _rootContainer = _uiDoc.rootVisualElement.Q<VisualElement>(ROOT_CONTAINER_UI_NAME);
            _dialogueLineContainer = _rootContainer.Q<VisualElement>(DIALOGUE_LINE_CONTAINER_UI_NAME);
            _dialogueBoxContainer = _rootContainer.Q<VisualElement>(DIALOGUE_BOX_CONTAINER_UI_NAME);
            _characterIcon = _rootContainer.Q<VisualElement>(CHARACTER_ICON_UI_NAME);
            _characterNameLabel = _rootContainer.Q<Label>(CHARACTER_NAME_UI_NAME);
            _richTextContainer = _rootContainer.Q<VisualElement>(RICH_TEXT_CONTAINER_UI_NAME);
            _richTextLabel = _rootContainer.Q<Label>(RICH_TEXT_LABEL_UI_NAME);
            _answersContainer = _rootContainer.Q<VisualElement>(ANSWERS_CONTAINER_UI_NAME);

            _rootContainer.AddToClassList(ROOT_CONTAINER_HIDDEN_CLASSNAME);
            HideDialogueLineContainer();
        }

        #endregion

        public void ProcessDialogue(DialogueSO dialogue)
        {
            _currentDialogue = dialogue;
            SetupRootContainer(dialogue);
            StartCoroutine(DisplayRootContainer());
            StartCoroutine(LoadNextDialogueLine(dialogue.DialogueTreeRoot));
        }

        private void OnDialogueLineClicked(ClickEvent _clickEvent)
        {
            // If no children, end of the dialogue
            if (!_currentDialogueLine.HasChildren())
            {
                // Add the quest if there is one
                if (_currentDialogueLine.Data.Quest != null)
                {
                    HeroTeamQuests.Instance.AddQuest(_currentDialogueLine.Data.Quest);
                }

                StartCoroutine(HideRootContainerAndEndDialogue());
                return;
            }

            // If children and answers, wait for answer click
            if (_currentDialogueLine.Data.Answers.Length > 0) return;

            // If children and no answers, display next dialogue line
            StartCoroutine(LoadNextDialogueLine(_currentDialogueLine.Children[0]));
        }

        private void OnAnswerClicked(ClickEvent clickEvent)
        {
            clickEvent.StopImmediatePropagation();

            // Get the index of the clicked answer
            Button clickedAnswer = clickEvent.currentTarget as Button;
            int answerIndex = _answersContainer.IndexOf(clickedAnswer.parent);

            // Get the next dialogue line based on the answer index
            StartCoroutine(LoadNextDialogueLine(_currentDialogueLine.Children[answerIndex]));
        }

        #region Static UI template references

        private static readonly string ROOT_CONTAINER_UI_NAME = "DialogueScreenRoot";
        private static readonly string DIALOGUE_LINE_CONTAINER_UI_NAME = "DialogueLineContainer";
        private static readonly string DIALOGUE_BOX_CONTAINER_UI_NAME = "DialogueBoxContainer";
        private static readonly string CHARACTER_ICON_UI_NAME = "CharacterIcon";
        private static readonly string CHARACTER_NAME_UI_NAME = "CharacterNameLabel";
        private static readonly string RICH_TEXT_CONTAINER_UI_NAME = "RichTextContainer";
        private static readonly string RICH_TEXT_LABEL_UI_NAME = "RichTextLabel";
        private static readonly string ANSWERS_CONTAINER_UI_NAME = "AnswersContainer";
        private static readonly string ANSWER_BUTTON_UI_NAME = "AnswerButton";

        private static readonly string ROOT_CONTAINER_HIDDEN_CLASSNAME = "dialogueScreenRootHidden";
        private static readonly string DIALOGUE_LINE_CONTAINER_BASE_CLASSNAME = "dialogueLineContainer";
        private static readonly string DIALOGUE_LINE_CONTAINER_HIDDEN_CLASSNAME = "dialogueLineContainerHidden";
        private static readonly string DIALOGUE_BOX_CONTAINER_BASE_CLASSNAME = "dialogueBoxContainer";
        private static readonly string CHARACTER_ICON_BASE_CLASSNAME = "characterIcon";
        private static readonly string RICH_TEXT_CONTAINER_BASE_CLASSNAME = "richTextContainer";
        private static readonly string ANSWERS_CONTAINER_BASE_CLASSNAME = "answersContainer";
        private static readonly string ANSWER_CONTAINER_ROOT_DEFAULT_CLASSNAME = "answerContainerRootDefault";
        private static readonly string ANSWER_CONTAINER_ROOT_HIDDEN_CLASSNAME = "answerContainerRootHidden";

        #endregion

        #region Dialogue UI setup

        private void SetupRootContainer(DialogueSO dialogue)
        {
            _rootContainer.RegisterCallback<ClickEvent>(OnDialogueLineClicked);
            if (dialogue.DialogueBackground != null)
                _rootContainer.style.backgroundImage = new StyleBackground(dialogue.DialogueBackground);
        }

        private IEnumerator LoadNextDialogueLine(TreeNode<DialogueLine> nextDialogueLine)
        {
            // Add the quest if there is one
            if (_currentDialogueLine != null && _currentDialogueLine.Data.Quest != null)
            {
                HeroTeamQuests.Instance.AddQuest(_currentDialogueLine.Data.Quest);
            }

            _previousDialogueLine = _currentDialogueLine;
            if (_previousDialogueLine != null &&
                nextDialogueLine.Data.Speaker != _previousDialogueLine.Data.Speaker)
            {
                HideDialogueLineContainer();
                yield return new WaitForSeconds(_timeBetweenDialogueLines);
            }

            _currentDialogueLine = nextDialogueLine;
            SetupDialogueLine(_currentDialogueLine.Data);
            DisplayDialogueLineContainer();
            yield return new WaitForSeconds(_timeBeforeAnswersDisplay);
            if (_currentDialogueLine.Data.Answers != null && _currentDialogueLine.Data.Answers.Length > 0)
                StartCoroutine(DisplayAnswers());
        }

        private void SetupDialogueLine(DialogueLine dialogueLine)
        {
            ClearAnswers();
            SetPanelOrientation(dialogueLine.IsRight);

            _characterIcon.style.backgroundImage = new StyleBackground(dialogueLine.Speaker.Icon);
            _characterNameLabel.text = dialogueLine.Speaker.Name;
            _richTextLabel.text = dialogueLine.RichText;

            if (dialogueLine.Answers != null && dialogueLine.Answers.Length > 0)
                foreach (string answer in dialogueLine.Answers)
                {
                    TemplateContainer answerButtonContainer = _answerContainerTemplate.Instantiate();
                    Button answerButton = answerButtonContainer.Q<Button>(ANSWER_BUTTON_UI_NAME);
                    answerButton.text = answer;
                    answerButton.RegisterCallback<ClickEvent>(OnAnswerClicked);
                    answerButtonContainer.AddToClassList(ANSWER_CONTAINER_ROOT_DEFAULT_CLASSNAME);
                    answerButtonContainer.AddToClassList(ANSWER_CONTAINER_ROOT_HIDDEN_CLASSNAME);
                    _answersContainer.Add(answerButtonContainer);
                }
        }

        private void SetPanelOrientation(bool isRight)
        {
            string oppositeOrientationSuffix = isRight ? "Left" : "Right";
            _dialogueLineContainer.RemoveFromClassList(DIALOGUE_LINE_CONTAINER_BASE_CLASSNAME +
                                                       oppositeOrientationSuffix);
            _dialogueBoxContainer.RemoveFromClassList(DIALOGUE_BOX_CONTAINER_BASE_CLASSNAME +
                                                      oppositeOrientationSuffix);
            _characterIcon.RemoveFromClassList(CHARACTER_ICON_BASE_CLASSNAME + oppositeOrientationSuffix);
            _richTextContainer.RemoveFromClassList(RICH_TEXT_CONTAINER_BASE_CLASSNAME + oppositeOrientationSuffix);
            _answersContainer.RemoveFromClassList(ANSWERS_CONTAINER_BASE_CLASSNAME + oppositeOrientationSuffix);

            string orientationSuffix = isRight ? "Right" : "Left";
            _dialogueLineContainer.AddToClassList(DIALOGUE_LINE_CONTAINER_BASE_CLASSNAME + orientationSuffix);
            _dialogueBoxContainer.AddToClassList(DIALOGUE_BOX_CONTAINER_BASE_CLASSNAME + orientationSuffix);
            _characterIcon.AddToClassList(CHARACTER_ICON_BASE_CLASSNAME + orientationSuffix);
            _richTextContainer.AddToClassList(RICH_TEXT_CONTAINER_BASE_CLASSNAME + orientationSuffix);
            _answersContainer.AddToClassList(ANSWERS_CONTAINER_BASE_CLASSNAME + orientationSuffix);
        }

        private void ClearAnswers()
        {
            _answersContainer.Clear();
        }

        #endregion

        #region Elements display/hide

        private IEnumerator DisplayRootContainer()
        {
            _rootContainer.RemoveFromClassList(ROOT_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(_timeBeforeFirstLineDisplay);
        }

        private IEnumerator HideRootContainer()
        {
            HideDialogueLineContainer();
            yield return new WaitForSeconds(_timeBetweenDialogueLines);
            _rootContainer.AddToClassList(ROOT_CONTAINER_HIDDEN_CLASSNAME);
        }

        private IEnumerator HideRootContainerAndEndDialogue()
        {
            StartCoroutine(HideRootContainer());
            yield return new WaitForSeconds(_timeBetweenDialogueLines);
            onDialogueEnded?.Invoke(_currentDialogue);
        }

        private void DisplayDialogueLineContainer()
        {
            _dialogueLineContainer.RemoveFromClassList(DIALOGUE_LINE_CONTAINER_HIDDEN_CLASSNAME);
        }

        private void HideDialogueLineContainer()
        {
            _dialogueLineContainer.AddToClassList(DIALOGUE_LINE_CONTAINER_HIDDEN_CLASSNAME);
            if (_currentDialogueLine != null && _currentDialogueLine.Data.Answers.Length > 0)
                StartCoroutine(HideAnswers());
        }

        private IEnumerator DisplayAnswers()
        {
            foreach (VisualElement answer in _answersContainer.Children())
            {
                answer.RemoveFromClassList(ANSWER_CONTAINER_ROOT_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(_timeBetweenAnswersDisplay);
            }
        }

        private IEnumerator HideAnswers()
        {
            foreach (VisualElement answer in _answersContainer.Children())
            {
                answer.AddToClassList(ANSWER_CONTAINER_ROOT_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(_timeBetweenAnswersDisplay);
            }
        }

        #endregion
    }
}