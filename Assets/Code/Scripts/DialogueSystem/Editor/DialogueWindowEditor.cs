using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Dialogues;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    public class DialogueEditorWindow : EditorWindow
    {
        private DialogueGraphView _graphView;
        private DialogueSO _currentDialogue;

        [MenuItem("Window/Dialogue Editor")]
        public static void OpenWindow()
        {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void CreateGUI()
        {
            // Load UXML Layout
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueEditor.uxml");
            visualTree.CloneTree(rootVisualElement);

            // Get UI Elements
            var graphContainer = rootVisualElement.Q<VisualElement>("GraphContainer");
            var objectField = rootVisualElement.Q<ObjectField>("DialogueObjectField");
            var saveButton = rootVisualElement.Q<Button>("SaveButton");

            // Initialize Graph View
            _graphView = new DialogueGraphView();
            _graphView.StretchToParentSize();
            graphContainer.Add(_graphView);

            // Setup Object Field
            objectField.objectType = typeof(DialogueSO);
            objectField.RegisterValueChangedCallback(evt =>
            {
                _currentDialogue = evt.newValue as DialogueSO;
                LoadDialogue(_currentDialogue);
            });

            // Save Button
            saveButton.clicked += SaveDialogue;
        }

        private void LoadDialogue(DialogueSO dialogue)
        {
            if (dialogue == null) return;

            _graphView.PopulateFromDialogue(dialogue);
        }

        private void SaveDialogue()
        {
            if (_currentDialogue == null) return;

            _graphView.ApplyToDialogue(_currentDialogue);
            EditorUtility.SetDirty(_currentDialogue);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
