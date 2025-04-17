using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightConditions;
using FrostfallSaga.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(PassiveAbilitySO))]
    public class PassiveAbilitySOEditor : Editor
    {
        private static readonly string EFFECTS_PROPERTY_NAME = "effects";

        private static readonly string CONDITIONS_PROPERTY_NAME = "activationConditions";
        private ReorderableList conditionsList;
        private SerializedProperty conditionsProperty;
        private ReorderableList effectsList;
        private SerializedProperty effectsProperty;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);

            conditionsProperty = serializedObject.FindProperty(CONDITIONS_PROPERTY_NAME);
            conditionsList =
                AbstractListEditorBuilder.BuildAbstractList<AFighterCondition>(serializedObject, conditionsProperty);
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