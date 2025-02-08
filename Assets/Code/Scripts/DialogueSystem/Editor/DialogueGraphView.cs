using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using FrostfallSaga.Core.Dialogues;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        public Action<DialogueNode> OnNodeSelected;
        public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

        public DialogueGraphView()
        {
            style.flexGrow = 1;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(CreateEntryPointNode());
        }

        private Node CreateEntryPointNode()
        {
            var node = new Node
            {
                title = "Start",
                capabilities = Capabilities.Movable
            };

            var outputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "Start";
            node.outputContainer.Add(outputPort);

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(100, 200, 150, 200));

            return node;
        }

        public DialogueNode CreateDialogueNode(DialogueLine data)
        {
            var node = new DialogueNode(data, OnNodeSelected);
            node.SetPosition(new Rect(200, 200, 250, 300));
            AddElement(node);
            return node;
        }

        public void ConnectNodes(DialogueNode parentNode, DialogueNode childNode)
        {
            if (parentNode == null || childNode == null)
                return;

            Edge edge = parentNode.OutputPort.ConnectTo(childNode.InputPort);
            AddElement(edge);
        }
    }
}
