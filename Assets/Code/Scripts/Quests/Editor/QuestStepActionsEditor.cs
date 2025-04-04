using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace FrostfallSaga.FFSEditor.Quests
{
    [CustomEditor(typeof(QuestStepActionsSO))]
    public class QuestStepActionsEditor : Editor
    {
        private static readonly string NON_DECISIVE_ACTIONS_PROPERTY_NAME = "NonDecisiveActions";

        private static readonly string DECISIVE_ACTIONS_PROPERTY_NAME = "DecisiveActions";
        private ReorderableList decisiveActionsList;
        private SerializedProperty decisiveActionsProperty;
        private ReorderableList nonDecisiveActionsList;
        private SerializedProperty nonDecisiveActionsProperty;

        private void OnEnable()
        {
            nonDecisiveActionsProperty = serializedObject.FindProperty(NON_DECISIVE_ACTIONS_PROPERTY_NAME);
            nonDecisiveActionsList =
                AbstractListEditorBuilder.BuildAbstractList<AQuestAction>(serializedObject, nonDecisiveActionsProperty);

            decisiveActionsProperty = serializedObject.FindProperty(DECISIVE_ACTIONS_PROPERTY_NAME);
            decisiveActionsList =
                AbstractListEditorBuilder.BuildAbstractList<AQuestAction>(serializedObject, decisiveActionsProperty);
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