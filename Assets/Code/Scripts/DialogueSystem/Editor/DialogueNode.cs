using System;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using FrostfallSaga.Core.Dialogues;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    public class DialogueNode : Node
    {
        public DialogueLine Data { get; private set; }
        public Action<DialogueNode> OnNodeSelected;

        private TextField _titleField;
        private TextField _richTextField;
        private VisualElement _answersContainer;

        // ✅ Make input & output ports public
        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        public string GUID { get; private set; } // Unique identifier

        public DialogueNode(DialogueLine data, Action<DialogueNode> onNodeSelected)
        {
            Data = data;
            OnNodeSelected = onNodeSelected;
            title = string.IsNullOrEmpty(data.Title) ? "Untitled" : data.Title;
            GUID = Guid.NewGuid().ToString(); // ✅ Generate a unique ID

            // Setup UI
            SetupPorts();
            SetupTitleField();
            SetupRichTextField();
            SetupAnswersContainer();
            RefreshExpandedState();
        }

        private void SetupPorts()
        {
            // ✅ Use Port.Create<Edge> instead of InstantiatePort
            InputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            InputPort.portName = "Input";
            inputContainer.Add(InputPort);

            OutputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            OutputPort.portName = "Output";
            outputContainer.Add(OutputPort);
        }

        private void SetupTitleField()
        {
            _titleField = new TextField("Title") { value = Data.Title };
            _titleField.RegisterValueChangedCallback(evt =>
            {
                Data.SetTitle(evt.newValue);
                title = evt.newValue;
            });
            mainContainer.Add(_titleField);
        }

        private void SetupRichTextField()
        {
            _richTextField = new TextField("Rich Text") { value = Data.RichText, multiline = true };
            _richTextField.RegisterValueChangedCallback(evt =>
            {
                Data.SetRichText(evt.newValue);
            });
            mainContainer.Add(_richTextField);
        }

        private void SetupAnswersContainer()
        {
            _answersContainer = new VisualElement();
            mainContainer.Add(_answersContainer);
            RefreshAnswers();
        }

        private void RefreshAnswers()
        {
            _answersContainer.Clear();
            if (Data.Answers != null)
            {
                for (int i = 0; i < Data.Answers.Length; i++)
                {
                    int index = i;
                    var answerField = new TextField($"Answer {index + 1}") { value = Data.Answers[index] };
                    answerField.RegisterValueChangedCallback(evt =>
                    {
                        Data.Answers[index] = evt.newValue;
                    });
                    _answersContainer.Add(answerField);
                }
            }
        }

        public void AddAnswer(string answerText)
        {
            var answersList = new System.Collections.Generic.List<string>(Data.Answers ?? new string[0])
            {
                answerText
            };
            Data.SetAnswers(answersList.ToArray());
            RefreshAnswers();
        }

        public void Select()
        {
            OnNodeSelected?.Invoke(this);
        }
    }
}
