using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Utils.Trees;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    public class DialogueNode : Node
    {
        public Action<DialogueNode> onAnswerAdded;
        public Action<DialogueNode> OnAddLineContinuation;
        public Action<DialogueNode> onNodeRemoved;
        public Action onNodeModified;

        public TreeNode<DialogueLine> TreeNode { get; private set; }
        public DialogueLine Data => TreeNode.GetData();
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        private Button _addAnswerButton;
        private Button _addContinuationButton;
        private VisualElement _answersContainer;
        private TextField _titleField;
        private TextField _richTextField;
        private ObjectField _speakerField;
        private Toggle _isRightToggle;

        public DialogueNode(TreeNode<DialogueLine> treeNode)
        {
            TreeNode = treeNode;

            title = Data.Title;
            style.width = 250;

            SetupPorts();
            SetupUI();
            RefreshNode();
        }

        private void SetupPorts()
        {
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(DialogueNode));
            InputPort.portName = "Input";
            inputContainer.Add(InputPort);

            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(DialogueNode));
            OutputPort.portName = "Output";
            outputContainer.Add(OutputPort);
        }

        private void SetupUI()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.style.paddingBottom = 5;

            _titleField = new TextField("Title") { value = Data.Title };
            _titleField.RegisterValueChangedCallback(evt => UpdateTitle(evt.newValue));
            container.Add(_titleField);

            _richTextField = new TextField("Text") { value = Data.RichText, multiline = true };
            _richTextField.RegisterValueChangedCallback(evt => UpdateRichText(evt.newValue));
            container.Add(_richTextField);

            _speakerField = new ObjectField("Speaker") { objectType = typeof(DialogueParticipantSO), value = Data.Speaker };
            _speakerField.RegisterValueChangedCallback(evt => UpdateSpeaker(evt.newValue as DialogueParticipantSO));
            container.Add(_speakerField);

            _isRightToggle = new Toggle("Is Right") { value = Data.IsRight };
            _isRightToggle.RegisterValueChangedCallback(evt => UpdateIsRight(evt.newValue));
            container.Add(_isRightToggle);

            _answersContainer = new VisualElement();
            _answersContainer.style.marginTop = 5;
            container.Add(_answersContainer);

            _addAnswerButton = new Button(() => onAnswerAdded?.Invoke(this)) { text = "Add Answer" };
            _addContinuationButton = new Button(() => OnAddLineContinuation?.Invoke(this)) { text = "Add Line Continuation" };

            container.Add(_addAnswerButton);
            container.Add(_addContinuationButton);

            var removeButton = new Button(() => onNodeRemoved?.Invoke(this)) { text = "Remove Node" };
            removeButton.style.marginTop = 5;
            container.Add(removeButton);

            mainContainer.Add(container);
        }

        private void UpdateTitle(string newTitle)
        {
            Data.SetTitle(newTitle);
            title = newTitle;
            onNodeModified?.Invoke();
        }

        private void UpdateRichText(string newText)
        {
            Data.SetRichText(newText);
            onNodeModified?.Invoke();
        }

        private void UpdateSpeaker(DialogueParticipantSO newSpeaker)
        {
            Data.SetSpeaker(newSpeaker);
            onNodeModified?.Invoke();
        }

        private void UpdateIsRight(bool newIsRight)
        {
            Data.SetIsRight(newIsRight);
            onNodeModified?.Invoke();
        }

        public void RefreshNode()
        {
            _answersContainer.Clear();

            if (Data.Answers != null)
            {
                foreach (var answer in Data.Answers)
                {
                    var answerField = new TextField("Answer") { value = answer };
                    answerField.RegisterValueChangedCallback(evt => UpdateAnswer(answer, evt.newValue));
                    _answersContainer.Add(answerField);
                }
            }

            _addAnswerButton.SetEnabled(TreeNode.GetChildren().Count == 0 || (Data.Answers != null && Data.Answers.Length > 0));
            _addContinuationButton.SetEnabled((Data.Answers == null || Data.Answers.Length == 0) && TreeNode.GetChildren().Count == 0);
        }

        private void UpdateAnswer(string oldAnswer, string newAnswer)
        {
            var answers = Data.Answers.ToList();
            int index = answers.IndexOf(oldAnswer);
            if (index != -1)
            {
                answers[index] = newAnswer;
                Data.SetAnswers(answers.ToArray());
                onNodeModified?.Invoke();
            }
        }
    }
}
