using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Utils.Trees;
using FrostfallSaga.Core.Quests;

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
        public int Depth { get; private set; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }
        public DialogueNode ParentNode { get; private set; }
        public List<DialogueNode> ChildrenNodes { get; private set; } = new List<DialogueNode>();

        private Button _addAnswerButton;
        private Button _addContinuationButton;
        private VisualElement _answersContainer;
        private TextField _titleField;
        private TextField _richTextField;
        private ObjectField _speakerField;
        private Toggle _isRightToggle;
        private ObjectField _questField;

        public DialogueNode(TreeNode<DialogueLine> treeNode, int depth, DialogueNode parentNode)
        {
            TreeNode = treeNode;
            Depth = depth;
            ParentNode = parentNode;

            title = Data.Title;
            style.width = 250;

            SetupPorts();
            SetupUI();
            RefreshNode();
        }

        public void AddChild(DialogueNode childNode)
        {
            ChildrenNodes.Add(childNode);
        }

        public void RemoveChild(DialogueNode childNode)
        {
            ChildrenNodes.Remove(childNode);
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

            _questField = new ObjectField("Quest") { objectType = typeof(AQuestSO), value = Data.Quest };
            _questField.RegisterValueChangedCallback(evt => UpdateQuest(evt.newValue as AQuestSO));
            container.Add(_questField);

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

        private void UpdateQuest(AQuestSO newQuest)
        {
            Data.SetQuest(newQuest);
            onNodeModified?.Invoke();
        }

        public void RefreshNode()
        {
            _answersContainer.Clear();

            if (Data.Answers != null)
            {
                foreach (string answer in Data.Answers)
                {
                    TextField answerField = new("Answer") { value = answer };
                    answerField.RegisterValueChangedCallback(evt => UpdateAnswer(answer, evt.newValue));
                    _answersContainer.Add(answerField);
                }
            }

            _addAnswerButton.SetEnabled(TreeNode.GetChildren().Count == 0 || (Data.Answers != null && Data.Answers.Length > 0));
            _addContinuationButton.SetEnabled((Data.Answers == null || Data.Answers.Length == 0) && TreeNode.GetChildren().Count == 0);
        }

        private void UpdateAnswer(string oldAnswer, string newAnswer)
        {
            List<string> answers = Data.Answers.ToList();
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
