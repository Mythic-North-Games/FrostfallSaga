using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Utils.Editor;
using FrostfallSaga.Core.Quests;

namespace FrostfallSaga.FFSEditor.Quests
{
    [CustomEditor(typeof(ActionsQuestSO))]
    public class ActionsQuestSOEditor : Editor
    {
        private readonly static string ACTIONS_PROPERTY_NAME = "Actions";
        private SerializedProperty actionsProperty;
        private ReorderableList actionsList;

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
