using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.Utils.Editor
{
    public class AbstractAttributeDrawer<T> : PropertyDrawer
    {
        private readonly float _buttonWidth = 60f; // Width of the buttons
        private readonly float _spacing = 2f; // Spacing between elements

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // If no instance is assigned, show the "Select" button
            if (property.managedReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                ShowSelectButton(position, property);
                EditorGUI.EndProperty();
                return;
            }

            // Draw the property field with children (if they exist)
            EditorGUI.PropertyField(position, property, label, true);

            // Draw the "Remove" and "Change" buttons regardless of visible children
            if (property.isExpanded || !property.hasVisibleChildren)
            {
                // Adjust position for drawing the buttons
                position.y += EditorGUI.GetPropertyHeight(property, label, true);

                ShowInstanceDetails(position, property);
            }

            EditorGUI.EndProperty();
        }

        private void ShowSelectButton(Rect position, SerializedProperty property)
        {
            // Display the "Select" button to choose a concrete class
            if (GUI.Button(
                    new Rect(position.x + position.width - _buttonWidth, position.y, _buttonWidth,
                        EditorGUIUtility.singleLineHeight), "Select")) ShowTypeSelector(property);
        }

        private void ShowInstanceDetails(Rect position, SerializedProperty property)
        {
            // Get the name of the instantiated class
            var instanceName = property.managedReferenceValue.GetType().Name;

            // Display the name of the current instance
            EditorGUI.LabelField(
                new Rect(position.x, position.y, position.width - _buttonWidth * 2, EditorGUIUtility.singleLineHeight),
                "Current: " + instanceName);

            // Draw the "Remove" button
            if (GUI.Button(
                    new Rect(position.x + position.width - _buttonWidth * 2, position.y, _buttonWidth,
                        EditorGUIUtility.singleLineHeight), "Remove"))
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            }

            // Draw the "Change" button
            if (GUI.Button(
                    new Rect(position.x + position.width - _buttonWidth, position.y, _buttonWidth,
                        EditorGUIUtility.singleLineHeight), "Change")) ShowTypeSelector(property);
        }

        private void ShowTypeSelector(SerializedProperty property)
        {
            GenericMenu menu = new();

            // Get all concrete types of T
            List<Type> spawnControllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract)
                .ToList();

            foreach (Type type in spawnControllerTypes)
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    // When a concrete class is selected, instantiate and assign it
                    T instance = (T)Activator.CreateInstance(type);
                    property.managedReferenceValue = instance;

                    // Ensure Unity reflects the change
                    property.serializedObject.ApplyModifiedProperties();
                });

            menu.ShowAsContext();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Get the height of the normal property
            var height = EditorGUI.GetPropertyHeight(property, label, true);

            // If an instance exists, add extra height for the instance details and buttons
            if (property.managedReferenceValue != null)
                height += EditorGUIUtility.singleLineHeight + _spacing; // Add space for "Remove" and "Change" buttons

            return height;
        }
    }
}