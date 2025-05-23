using System;
using System.Collections;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils.DataStructures.TreeNode;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.Dialogues
{
    public class DialogueUIProcessor : BaseUIController
    {
        private static readonly string HIGH_MARGIN_BOTTOM_LETTERS = "g";

        #region Static UI template references
        private static readonly string ROOT_CONTAINER_UI_NAME = "DialogueScreenRoot";
        private static readonly string DIALOGUE_LINE_CONTAINER_UI_NAME = "DialogueLineContainer";
        private static readonly string CHARACTER_ICON_UI_NAME = "CharacterIcon";
        private static readonly string DIALOGUE_BOX_CONTAINER_UI_NAME = "DialogueBoxContainer";
        private static readonly string CHARACTER_NAME_CONTAINER_UI_NAME = "CharacterNameContainer";
        private static readonly string CHARACTER_NAME_UI_NAME = "CharacterNameLabel";
        private static readonly string RICH_TEXT_LABEL_UI_NAME = "RichTextLabel";
        private static readonly string ANSWERS_CONTAINER_UI_NAME = "AnswersContainer";

        private static readonly string ROOT_CONTAINER_HIDDEN_CLASSNAME = "dialogueScreenRootHidden";
        private static readonly string DIALOGUE_LINE_CONTAINER_BASE_CLASSNAME = "dialogueLineContainer";
        private static readonly string CONTAINER_HIDDEN_CLASSNAME = "containerHidden";
        private static readonly string CHARACTER_ICON_BASE_CLASSNAME = "characterIcon";
        private static readonly string DIALOGUE_BOX_CONTAINER_BASE_CLASSNAME = "dialogueBoxContainer";
        private static readonly string CHARACTER_NAME_CONTAINER_BASE_CLASSNAME = "characterNameContainer";
        private static readonly string CHARACTER_NAME_LABEL_EXTRA_MARGIN_BOTTOM_CLASSNAME = "characterNameLabelExtraMarginBottom";
        private static readonly string ANSWERS_CONTAINER_BASE_CLASSNAME = "answersContainer";
        private static readonly string ANSWER_CONTAINER_ROOT_DEFAULT_CLASSNAME = "answerContainerRootDefault";
        private static readonly string ANSWER_CONTAINER_ROOT_HIDDEN_CLASSNAME = "answerContainerRootHidden";
        #endregion

        [SerializeField] private VisualTreeAsset _answerContainerTemplate;
        [SerializeField] private float _timeBeforeFirstLineDisplay = 1f;
        [SerializeField] private float _timeBetweenDialogueLines = 0.5f;
        [SerializeField] private float _timeBeforeAnswersDisplay = 0.5f;
        [SerializeField] private float _timeBetweenAnswersDisplay = 0.2f;
        [SerializeField] private float _timeBetweenContainersDisplay = 0.1f;

        public Action<DialogueSO> onDialogueEnded;

        private VisualElement _rootContainer;
        private VisualElement _dialogueLineContainer;
        private VisualElement _characterIcon;
        private VisualElement _dialogueBoxContainer;
        private VisualElement _characterNameContainer;
        private Label _characterNameLabel;
        private Label _richTextLabel;
        private VisualElement _answersContainer;

        private DialogueSO _currentDialogue;
        private TreeNode<DialogueLine> _currentDialogueLine;
        private TreeNode<DialogueLine> _previousDialogueLine;

        #region Setup

        private void Awake()
        {
            _rootContainer = _uiDoc.rootVisualElement.Q<VisualElement>(ROOT_CONTAINER_UI_NAME);
            _dialogueLineContainer = _rootContainer.Q<VisualElement>(DIALOGUE_LINE_CONTAINER_UI_NAME);
            _characterIcon = _rootContainer.Q<VisualElement>(CHARACTER_ICON_UI_NAME);
            _dialogueBoxContainer = _rootContainer.Q<VisualElement>(DIALOGUE_BOX_CONTAINER_UI_NAME);
            _characterNameContainer = _rootContainer.Q<VisualElement>(CHARACTER_NAME_CONTAINER_UI_NAME);
            _characterNameLabel = _rootContainer.Q<Label>(CHARACTER_NAME_UI_NAME);
            _richTextLabel = _rootContainer.Q<Label>(RICH_TEXT_LABEL_UI_NAME);
            _answersContainer = _rootContainer.Q<VisualElement>(ANSWERS_CONTAINER_UI_NAME);

            _rootContainer.AddToClassList(ROOT_CONTAINER_HIDDEN_CLASSNAME);
            _dialogueBoxContainer.AddToClassList(CONTAINER_HIDDEN_CLASSNAME);
            _characterIcon.AddToClassList(CONTAINER_HIDDEN_CLASSNAME);
            _answersContainer.Clear();
        }

        #endregion

        public void ProcessDialogue(DialogueSO dialogue)
        {
            _currentDialogue = dialogue;
            SetupRootContainer(dialogue);
            StartCoroutine(LoadNextDialogueLine(dialogue.DialogueTreeRoot, true));
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

        private void OnAnswerClicked(LeftArrowButtonUIController clickedAnswerController)
        {
            // Get the index of the clicked answer
            int answerIndex = _answersContainer.IndexOf(clickedAnswerController.Root);

            // Get the next dialogue line based on the answer index
            StartCoroutine(LoadNextDialogueLine(_currentDialogueLine.Children[answerIndex]));
        }

        #region Dialogue UI setup

        private void SetupRootContainer(DialogueSO dialogue)
        {
            _rootContainer.RegisterCallback<ClickEvent>(OnDialogueLineClicked);
            if (dialogue.DialogueBackground != null)
                _rootContainer.style.backgroundImage = new StyleBackground(dialogue.DialogueBackground);
        }

        private IEnumerator LoadNextDialogueLine(TreeNode<DialogueLine> nextDialogueLine, bool firstLine = false)
        {
            // If first line, display the root container as well and wait for a bit
            if (firstLine)
            {
                _rootContainer.RemoveFromClassList(ROOT_CONTAINER_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(_timeBeforeFirstLineDisplay);
            }

            // Add the quest if there is one
            if (_currentDialogueLine != null && _currentDialogueLine.Data.Quest != null)
            {
                HeroTeamQuests.Instance.AddQuest(_currentDialogueLine.Data.Quest);
            }

            _previousDialogueLine = _currentDialogueLine;
            if (_previousDialogueLine != null &&
                nextDialogueLine.Data.Speaker != _previousDialogueLine.Data.Speaker)
            {
                StartCoroutine(HideDialogueLineContainer());
                yield return new WaitForSeconds(_timeBetweenDialogueLines);
            }

            _currentDialogueLine = nextDialogueLine;
            SetupDialogueLine(_currentDialogueLine.Data);
            yield return new WaitForSeconds(_timeBetweenAnswersDisplay);
            StartCoroutine(DisplayDialogueLineContainer());
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
            AdjustCharacterNameLabelMargin();
            _richTextLabel.text = dialogueLine.RichText;

            if (dialogueLine.Answers != null)
            {
                foreach (string answerText in dialogueLine.Answers)
                {
                    VisualElement answerButtonContainer = _answerContainerTemplate.Instantiate();
                    LeftArrowButtonUIController answerButtonUIController = new(answerButtonContainer, answerText);
                    answerButtonUIController.onButtonClicked += OnAnswerClicked;
                    answerButtonContainer.AddToClassList(ANSWER_CONTAINER_ROOT_DEFAULT_CLASSNAME);
                    answerButtonContainer.AddToClassList(ANSWER_CONTAINER_ROOT_HIDDEN_CLASSNAME);
                    _answersContainer.Add(answerButtonContainer);
                }
            }
        }

        private void AdjustCharacterNameLabelMargin()
        {
            foreach (char letter in _characterNameLabel.text)
            {
                if (HIGH_MARGIN_BOTTOM_LETTERS.Contains(letter))
                {
                    _characterNameLabel.AddToClassList(CHARACTER_NAME_LABEL_EXTRA_MARGIN_BOTTOM_CLASSNAME);
                    return;
                }
            }
            _characterNameLabel.RemoveFromClassList(CHARACTER_NAME_LABEL_EXTRA_MARGIN_BOTTOM_CLASSNAME);
        }

        private void SetPanelOrientation(bool isRight)
        {
            string oppositeOrientationSuffix = isRight ? "Left" : "Right";
            _dialogueLineContainer.RemoveFromClassList(DIALOGUE_LINE_CONTAINER_BASE_CLASSNAME + oppositeOrientationSuffix);
            _dialogueBoxContainer.RemoveFromClassList(DIALOGUE_BOX_CONTAINER_BASE_CLASSNAME + oppositeOrientationSuffix);
            _characterNameContainer.RemoveFromClassList(CHARACTER_NAME_CONTAINER_BASE_CLASSNAME + oppositeOrientationSuffix);
            _characterIcon.RemoveFromClassList(CHARACTER_ICON_BASE_CLASSNAME + oppositeOrientationSuffix);
            _answersContainer.RemoveFromClassList(ANSWERS_CONTAINER_BASE_CLASSNAME + oppositeOrientationSuffix);

            string orientationSuffix = isRight ? "Right" : "Left";
            _dialogueLineContainer.AddToClassList(DIALOGUE_LINE_CONTAINER_BASE_CLASSNAME + orientationSuffix);
            _dialogueBoxContainer.AddToClassList(DIALOGUE_BOX_CONTAINER_BASE_CLASSNAME + orientationSuffix);
            _characterNameContainer.AddToClassList(CHARACTER_NAME_CONTAINER_BASE_CLASSNAME + orientationSuffix);
            _characterIcon.AddToClassList(CHARACTER_ICON_BASE_CLASSNAME + orientationSuffix);
            _answersContainer.AddToClassList(ANSWERS_CONTAINER_BASE_CLASSNAME + orientationSuffix);
        }

        private void ClearAnswers()
        {
            _answersContainer.Clear();
        }

        #endregion

        #region Elements display/hide

        private IEnumerator HideRootContainer()
        {
            StartCoroutine(HideDialogueLineContainer());
            yield return new WaitForSeconds(_timeBetweenDialogueLines);
            _rootContainer.AddToClassList(ROOT_CONTAINER_HIDDEN_CLASSNAME);
        }

        private IEnumerator HideRootContainerAndEndDialogue()
        {
            StartCoroutine(HideRootContainer());
            yield return new WaitForSeconds(_timeBetweenDialogueLines);
            onDialogueEnded?.Invoke(_currentDialogue);
        }

        private IEnumerator DisplayDialogueLineContainer()
        {
            _characterIcon.RemoveFromClassList(CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(_timeBetweenContainersDisplay);
            _dialogueBoxContainer.RemoveFromClassList(CONTAINER_HIDDEN_CLASSNAME);
        }

        private IEnumerator HideDialogueLineContainer()
        {
            _dialogueBoxContainer.AddToClassList(CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(_timeBetweenContainersDisplay);
            _characterIcon.AddToClassList(CONTAINER_HIDDEN_CLASSNAME);
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