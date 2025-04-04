using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells.Impediments;
using FrostfallSaga.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(TrapSO))]
    public class TrapSOEditor : Editor
    {
        private static readonly string EFFECTS_PROPERTY_NAME = "Effects";
        private ReorderableList effectsList;
        private SerializedProperty effectsProperty;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, EFFECTS_PROPERTY_NAME);

            EditorGUILayout.Space();
            effectsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}