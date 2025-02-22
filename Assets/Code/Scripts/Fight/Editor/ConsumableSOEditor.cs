using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Utils.Editor;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(ConsumableSO))]
    public class ConsumableEditor : Editor
    {
        private readonly static string EFFECTS_PROPERTY_NAME = "Effects";
        private SerializedProperty effectsProperty;
        private ReorderableList effectsList;

        private void OnEnable()
        {
            effectsProperty = serializedObject.FindProperty(EFFECTS_PROPERTY_NAME);
            effectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(serializedObject, effectsProperty);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(
                serializedObject,
                EFFECTS_PROPERTY_NAME
            );

            EditorGUILayout.Space();
            effectsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
