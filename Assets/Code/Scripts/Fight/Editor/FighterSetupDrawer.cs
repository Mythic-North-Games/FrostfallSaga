using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using FrostfallSaga.Fight;
using FrostfallSaga.Utils.Editor;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomPropertyDrawer(typeof(FighterSetup))]
    public class FighterSetupDrawer : PropertyDrawer
    {
        private float expandedHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Reset expandedHeight for fresh computation
            expandedHeight = EditorGUIUtility.singleLineHeight; // Start with foldout height
            EditorGUI.BeginProperty(position, label, property);

            // Draw the foldout
            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label
            );

            // Track current position for drawing fields
            float currentY = position.y + EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Draw each property of FighterSetup
                currentY = DrawPropertyField(position, property, "fighterPrefab", currentY);
                currentY = DrawPropertyField(position, property, "name", currentY);
                currentY = DrawPropertyField(position, property, "sessionId", currentY);
                currentY = DrawPropertyField(position, property, "icon", currentY);
                currentY = DrawPropertyField(position, property, "diamondIcon", currentY);
                currentY = DrawPropertyField(position, property, "initialStats", currentY);
                currentY = DrawPropertyField(position, property, "fighterClass", currentY);
                currentY = DrawPropertyField(position, property, "personalityTrait", currentY);
                currentY = DrawPropertyField(position, property, "inventory", currentY);
                currentY = DrawPropertyField(position, property, "activeAbilities", currentY);
                currentY = DrawPropertyField(position, property, "passiveAbilities", currentY);
                currentY = DrawPropertyField(position, property, "receiveDamageAnimationName", currentY);
                currentY = DrawPropertyField(position, property, "healSelfAnimationName", currentY);
                currentY = DrawPropertyField(position, property, "reduceStatAnimationName", currentY);
                DrawPropertyField(position, property, "increaseStatAnimationName", currentY);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Use expandedHeight computed during OnGUI for accurate inspector height
            return property.isExpanded ? expandedHeight : EditorGUIUtility.singleLineHeight;
        }

        private float DrawPropertyField(Rect position, SerializedProperty property, string propertyName, float currentY)
        {
            SerializedProperty subProperty = property.FindPropertyRelative(propertyName);

            if (subProperty != null)
            {
                float propertyHeight = EditorGUI.GetPropertyHeight(subProperty, true);
                EditorGUI.PropertyField(
                    new Rect(position.x, currentY, position.width, propertyHeight),
                    subProperty,
                    true
                );

                // Increment expandedHeight and currentY
                expandedHeight += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
                currentY += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return currentY;
        }

        private float BuildAndDrawReorderableList<T>(Rect fieldRect, SerializedProperty property)
        {
            // Create and render the ReorderableList
            ReorderableList reorderableList = AbstractListEditorBuilder.BuildAbstractList<T>(
                property.serializedObject, property
            );

            float listHeight = reorderableList.GetHeight();
            reorderableList.DoList(new Rect(fieldRect.x, fieldRect.y, fieldRect.width, listHeight));

            // Adjust expandedHeight and return new y position
            expandedHeight += listHeight + EditorGUIUtility.standardVerticalSpacing;
            return fieldRect.y + listHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
