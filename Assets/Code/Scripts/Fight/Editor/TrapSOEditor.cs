using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Utils.Editor;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells.Impediments;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(TrapSO))]
    public class TrapSOEditor : Editor
    {
        private readonly static string EFFECTS_PROPERTY_NAME = "OtherCellsEffects";
        private readonly static string EFFECTS_PROPERTY_NAME2 = "OnCellEffects";
        private readonly static string CELL_ALTERATIONS_PROPERTY_NAME = "CellAlterations";

        private SerializedProperty effectsProperty;
        private ReorderableList effectsList;

        private SerializedProperty effectsProperty2;
        private ReorderableList effectsList2;

        private SerializedProperty cellAlterationsProperty;
        private ReorderableList cellAlterationsList;
        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);
            effectsProperty2 = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME2);
            effectsList2 = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty2);
            cellAlterationsProperty = serializedObject.FindProperty(CELL_ALTERATIONS_PROPERTY_NAME);
            cellAlterationsList = AbstractListEditorBuilder.BuildAbstractList<AFightCellAlteration>(serializedObject, cellAlterationsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, EFFECTS_PROPERTY_NAME, EFFECTS_PROPERTY_NAME2, CELL_ALTERATIONS_PROPERTY_NAME);

            EditorGUILayout.Space();
            effectsList.DoLayoutList();
            EditorGUILayout.Space();

            effectsList2.DoLayoutList();
            EditorGUILayout.Space();

            cellAlterationsList.DoLayoutList();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
