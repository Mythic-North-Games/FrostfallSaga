using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace FrostfallSaga.FFSEditor.Quests
{
    [CustomEditor(typeof(ActionsQuestSO))]
    public class ActionsQuestSOEditor : Editor
    {
        private static readonly string ACTIONS_PROPERTY_NAME = "Actions";
        private ReorderableList actionsList;
        private SerializedProperty actionsProperty;

        private void OnEnable()
        {
            actionsProperty = serializedObject.FindProperty(ACTIONS_PROPERTY_NAME);
            actionsList = AbstractListEditorBuilder.BuildAbstractList<AQuestAction>(serializedObject, actionsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(
                serializedObject,
                ACTIONS_PROPERTY_NAME
            );

            EditorGUILayout.Space();
            actionsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}