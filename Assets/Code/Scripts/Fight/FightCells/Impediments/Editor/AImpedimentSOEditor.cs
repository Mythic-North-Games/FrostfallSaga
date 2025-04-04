using FrostfallSaga.Fight.FightCells.Impediments;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(AImpedimentSO), true)]
    public class AImpedimentSOEditor : Editor
    {
        private static readonly string SPAWN_CONTROLLER_PROPERTY_NAME = "SpawnController";

        private static readonly string DESTROY_CONTROLLER_PROPERTY_NAME = "DestroyController";
        private SerializedProperty destroyControllerProperty;
        private SerializedProperty spawnControllerProperty;

        private void OnEnable()
        {
            spawnControllerProperty = serializedObject.FindProperty(SPAWN_CONTROLLER_PROPERTY_NAME);
            destroyControllerProperty = serializedObject.FindProperty(DESTROY_CONTROLLER_PROPERTY_NAME);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw default inspector fields
            DrawPropertiesExcluding(serializedObject, SPAWN_CONTROLLER_PROPERTY_NAME, DESTROY_CONTROLLER_PROPERTY_NAME);

            EditorGUILayout.Space();

            // Draw custom SpawnController and DestroyController
            EditorGUILayout.PropertyField(
                spawnControllerProperty,
                new GUIContent(SPAWN_CONTROLLER_PROPERTY_NAME),
                true
            );
            EditorGUILayout.PropertyField(
                destroyControllerProperty,
                new GUIContent(DESTROY_CONTROLLER_PROPERTY_NAME),
                true
            );

            serializedObject.ApplyModifiedProperties();
        }
    }
}