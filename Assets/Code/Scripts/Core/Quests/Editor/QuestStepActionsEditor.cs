using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Utils.Editor;
using FrostfallSaga.Core.Quests;

namespace FrostfallSaga.FFSEditor.Core.Quests
{
    [CustomEditor(typeof(QuestStepActionsSO))]
    public class QuestStepActionsEditor : Editor
    {
        private readonly static string NON_DECISIVE_ACTIONS_PROPERTY_NAME = "NonDecisiveActions";
        private SerializedProperty nonDecisiveActionsProperty;
        private ReorderableList nonDecisiveActionsList;

        private readonly static string DECISIVE_ACTIONS_PROPERTY_NAME = "DecisiveActions";
        private SerializedProperty decisiveActionsProperty;
        private ReorderableList decisiveActionsList;

        private void OnEnable()
        {
            nonDecisiveActionsProperty = serializedObject.FindProperty(NON_DECISIVE_ACTIONS_PROPERTY_NAME);
            nonDecisiveActionsList = AbstractListEditorBuilder.BuildAbstractList<AQuestAction>(serializedObject, nonDecisiveActionsProperty);

            decisiveActionsProperty = serializedObject.FindProperty(DECISIVE_ACTIONS_PROPERTY_NAME);
            decisiveActionsList = AbstractListEditorBuilder.BuildAbstractList<AQuestAction>(serializedObject, decisiveActionsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(
                serializedObject,
                NON_DECISIVE_ACTIONS_PROPERTY_NAME,
                DECISIVE_ACTIONS_PROPERTY_NAME
            );

            EditorGUILayout.Space();
            nonDecisiveActionsList.DoLayoutList();

            EditorGUILayout.Space();
            decisiveActionsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
