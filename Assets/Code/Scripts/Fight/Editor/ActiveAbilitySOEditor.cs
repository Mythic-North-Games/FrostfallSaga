using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Editors;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(ActiveAbilitySO))]
    public class ActiveAbilitySOEditor : Editor
    {
        private readonly static string EFFECTS_PROPERTY_NAME = "Effects";
        private SerializedProperty effectsProperty;
        private ReorderableList effectsList;

        private readonly static string MASTERSTOKE_EFFECTS_PROPERTY_NAME = "MasterstrokeEffects";
        private SerializedProperty masterstrokeEffectsProperty;
        private ReorderableList masterstrokeEffectsList;

        private readonly static string CELL_ALTERATIONS_PROPERTY_NAME = "CellAlterations";
        private SerializedProperty cellAlterationsProperty;
        private ReorderableList cellAlterationsList;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);

            masterstrokeEffectsProperty = serializedObject.FindProperty(MASTERSTOKE_EFFECTS_PROPERTY_NAME);
            masterstrokeEffectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, masterstrokeEffectsProperty);

            cellAlterationsProperty = serializedObject.FindProperty(CELL_ALTERATIONS_PROPERTY_NAME);
            cellAlterationsList = AbstractListEditorBuilder.BuildAbstractList<AFightCellAlteration>(serializedObject, cellAlterationsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(
                serializedObject,
                EFFECTS_PROPERTY_NAME,
                MASTERSTOKE_EFFECTS_PROPERTY_NAME,
                CELL_ALTERATIONS_PROPERTY_NAME
            );

            EditorGUILayout.Space();
            effectsList.DoLayoutList();

            EditorGUILayout.Space();
            masterstrokeEffectsList.DoLayoutList();

            EditorGUILayout.Space();
            cellAlterationsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}