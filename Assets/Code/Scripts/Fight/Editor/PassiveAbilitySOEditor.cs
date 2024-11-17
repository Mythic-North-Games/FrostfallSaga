using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Editors;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightConditions;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(PassiveAbilitySO))]
    public class PassiveAbilitySOEditor : Editor
    {
        private readonly static string EFFECTS_PROPERTY_NAME = "Effects";
        private SerializedProperty effectsProperty;
        private ReorderableList effectsList;

        private readonly static string CONDITIONS_PROPERTY_NAME = "ActivationConditions";
        private SerializedProperty conditionsProperty;
        private ReorderableList conditionsList;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);

            conditionsProperty = serializedObject.FindProperty(CONDITIONS_PROPERTY_NAME);
            conditionsList = AbstractListEditorBuilder.BuildAbstractList<AFighterCondition>(serializedObject, conditionsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, EFFECTS_PROPERTY_NAME, CONDITIONS_PROPERTY_NAME);

            EditorGUILayout.Space();
            effectsList.DoLayoutList();

            EditorGUILayout.Space();
            conditionsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
