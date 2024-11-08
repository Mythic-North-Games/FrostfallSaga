using UnityEditor;
using UnityEditorInternal;
using FrostfallSaga.Editors;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.KingdomToFight;

namespace FrostfallSaga.FFSEditor.KingdomToFight
{
    [CustomEditor(typeof(FighterConfigurationSO))]
    public class ActiveAbilitySOEditor : Editor
    {
        private readonly static string DIRECT_ATTACK_EFFECTS_PROPERTY_NAME = "DirectAttackEffects";
        private SerializedProperty directAttackEffectsProperty;
        private ReorderableList directAttackEffectsList;

        private void OnEnable()
        {
            directAttackEffectsProperty = serializedObject.FindProperty(DIRECT_ATTACK_EFFECTS_PROPERTY_NAME);
            directAttackEffectsList = AbstractListEditorBuilder.BuildAbstractList<AEffect>(
                serializedObject, directAttackEffectsProperty
            );
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, DIRECT_ATTACK_EFFECTS_PROPERTY_NAME);

            EditorGUILayout.Space();
            directAttackEffectsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
