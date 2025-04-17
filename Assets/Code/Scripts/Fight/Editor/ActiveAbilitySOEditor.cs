using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(ActiveAbilitySO))]
    public class ActiveAbilitySOEditor : Editor
    {
        private static readonly string EFFECTS_PROPERTY_NAME = "effects";

        private static readonly string MASTERSTOKE_EFFECTS_PROPERTY_NAME = "masterstrokeEffects";

        private static readonly string CELL_ALTERATIONS_PROPERTY_NAME = "cellAlterations";
        private ReorderableList cellAlterationsList;
        private SerializedProperty cellAlterationsProperty;
        private ReorderableList effectsList;
        private SerializedProperty effectsProperty;
        private ReorderableList masterstrokeEffectsList;
        private SerializedProperty masterstrokeEffectsProperty;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);

            masterstrokeEffectsProperty = serializedObject.FindProperty(MASTERSTOKE_EFFECTS_PROPERTY_NAME);
            masterstrokeEffectsList =
                AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, masterstrokeEffectsProperty);

            cellAlterationsProperty = serializedObject.FindProperty(CELL_ALTERATIONS_PROPERTY_NAME);
            cellAlterationsList =
                AbstractListEditorBuilder.BuildAbstractList<AFightCellAlteration>(serializedObject,
                    cellAlterationsProperty);
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