using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Trees;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    public class DialogueNode : Node
    {
        public Action<DialogueNode> OnAddLineContinuation;
        public Action<DialogueNode> OnAnswerAdded;
        public Action OnNodeModified;
        public Action<DialogueNode> OnNodeRemoved;

        private Button _addAnswerButton;
        private Button _addContinuationButton;
        private VisualElement _answersContainer;
        private Toggle _isRightToggle;
        private TextField _richTextField;
        private ObjectField _speakerField;
        private TextField _titleField;
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

        public TreeNode<DialogueLine> TreeNode { get; }
        public DialogueLine Data => TreeNode.GetData();
        public int Depth { get; private set; }
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }
        public DialogueNode ParentNode { get; private set; }
        public List<DialogueNode> ChildrenNodes { get; } = new();

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
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single,
                typeof(DialogueNode));
            InputPort.portName = "Input";
            inputContainer.Add(InputPort);

            OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                typeof(DialogueNode));
            OutputPort.portName = "Output";
            outputContainer.Add(OutputPort);
        }

        private void SetupUI()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Column;
            container.style.paddingBottom = 5;

            _titleField = new TextField("Title") { value = Data.Title };
            _titleField.RegisterValueChangedCallback(evt => UpdateTitle(evt.newValue));
            container.Add(_titleField);

            _richTextField = new TextField("Text") { value = Data.RichText, multiline = true };
            _richTextField.RegisterValueChangedCallback(evt => UpdateRichText(evt.newValue));
            container.Add(_richTextField);

            _speakerField = new ObjectField("Speaker")
                { objectType = typeof(DialogueParticipantSO), value = Data.Speaker };
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

            _addAnswerButton = new Button(() => OnAnswerAdded?.Invoke(this)) { text = "Add Answer" };
            _addContinuationButton = new Button(() => OnAddLineContinuation?.Invoke(this))
                { text = "Add Line Continuation" };

            container.Add(_addAnswerButton);
            container.Add(_addContinuationButton);

            Button removeButton = new(() => OnNodeRemoved?.Invoke(this)) { text = "Remove Node" };
            removeButton.style.marginTop = 5;
            container.Add(removeButton);

            mainContainer.Add(container);
        }

        private void UpdateTitle(string newTitle)
        {
            Data.SetTitle(newTitle);
            title = newTitle;
            OnNodeModified?.Invoke();
        }

        private void UpdateRichText(string newText)
        {
            Data.SetRichText(newText);
            OnNodeModified?.Invoke();
        }

        private void UpdateSpeaker(DialogueParticipantSO newSpeaker)
        {
            Data.SetSpeaker(newSpeaker);
            OnNodeModified?.Invoke();
        }

        private void UpdateIsRight(bool newIsRight)
        {
            Data.SetIsRight(newIsRight);
            OnNodeModified?.Invoke();
        }

        private void UpdateQuest(AQuestSO newQuest)
        {
            Data.SetQuest(newQuest);
            OnNodeModified?.Invoke();
        }

        public void RefreshNode()
        {
            _answersContainer.Clear();

            if (Data.Answers != null)
                foreach (string answer in Data.Answers)
                {
                    TextField answerField = new("Answer") { value = answer };
                    answerField.RegisterValueChangedCallback(evt => UpdateAnswer(answer, evt.newValue));
                    _answersContainer.Add(answerField);
                }

            _addAnswerButton.SetEnabled(TreeNode.GetChildren().Count == 0 ||
                                        (Data.Answers != null && Data.Answers.Length > 0));
            _addContinuationButton.SetEnabled((Data.Answers == null || Data.Answers.Length == 0) &&
                                              TreeNode.GetChildren().Count == 0);
        }

        private void UpdateAnswer(string oldAnswer, string newAnswer)
        {
            List<string> answers = Data.Answers.ToList();
            int index = answers.IndexOf(oldAnswer);
            if (index != -1)
            {
                answers[index] = newAnswer;
                Data.SetAnswers(answers.ToArray());
                OnNodeModified?.Invoke();
            }
        }
    }
}