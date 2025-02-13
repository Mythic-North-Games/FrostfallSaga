using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using FrostfallSaga.Core.Dialogues;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueGraphView _graphView;
        private DialogueSO _currentDialogue;
        private ObjectField _dialogueField;
        private Editor _inspectorPanel;

        [MenuItem("Window/CustomTools/Dialogue Editor")]
        public static void OpenWindow()
        {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.minSize = new Vector2(1000, 600);
            window.Show();
        }

        private void CreateGUI()
        {
            // Root Layout: Horizontal Split
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Row;

            // Left Side: Graph View
            _graphView = new DialogueGraphView
            {
                style = { flexGrow = 1 } // Takes most space
            };
            root.Add(_graphView);

            // Right Side: Vertical Controls & Inspector
            var rightPanel = new VisualElement
            {
                style =
                {
                    width = 300,
                    flexDirection = FlexDirection.Column,
                    paddingLeft = 10,
                    paddingRight = 10
                }
            };
            root.Add(rightPanel);

            // === TOP: Dialogue Controls ===
            var controlPanel = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    paddingBottom = 10
                }
            };
            rightPanel.Add(controlPanel);

            // Create New Dialogue Button
            var newButton = new Button(CreateNewDialogue)
            {
                text = "Create New Dialogue"
            };
            controlPanel.Add(newButton);

            // Load Dialogue Field
            _dialogueField = new ObjectField("Load Dialogue")
            {
                objectType = typeof(DialogueSO)
            };
            _dialogueField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null)
                {
                    _graphView.ClearGraph();

                    return;
                }
                _currentDialogue = evt.newValue as DialogueSO;
                LoadDialogue(_currentDialogue);
            });
            controlPanel.Add(_dialogueField);

            // === MIDDLE: Separator ===
            var separator = new VisualElement
            {
                style =
                {
                    borderTopWidth = 5,
                    borderTopColor = new Color(0.5f, 0.5f, 0.5f),
                    marginTop = 10,
                    marginBottom = 10
                }
            };
            rightPanel.Add(separator);

            // === BOTTOM: Inspector Panel ===
            IMGUIContainer inspectorContainer = new(() =>
            {
                if (_inspectorPanel != null)
                {
                    _inspectorPanel.OnInspectorGUI();
                }
            });
            rightPanel.Add(inspectorContainer);
        }

        private void CreateNewDialogue()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save New Dialogue",
                "NewDialogue",
                "asset",
                "Choose a location to save the new DialogueSO"
            );

            if (string.IsNullOrEmpty(path)) return;

            DialogueSO newDialogue = ScriptableObject.CreateInstance<DialogueSO>();
            AssetDatabase.CreateAsset(newDialogue, path);
            AssetDatabase.SaveAssets();

            _currentDialogue = newDialogue;
            _dialogueField.value = newDialogue;
            LoadDialogue(newDialogue);
        }

        private void LoadDialogue(DialogueSO dialogue)
        {
            if (dialogue == null) return;
            _graphView.PopulateFromDialogue(dialogue);
            UpdateInspector(dialogue);
        }

        private void UpdateInspector(DialogueSO dialogue)
        {
            if (dialogue == null) return;
            _inspectorPanel = Editor.CreateEditor(dialogue);
            if (_inspectorPanel is DialogueSOEditor dialogueInspector)
            {
                dialogueInspector.onDialogueChanged += () => _graphView.PopulateFromDialogue(dialogue);
            }
            rootVisualElement.MarkDirtyRepaint();
        }
    }
}
