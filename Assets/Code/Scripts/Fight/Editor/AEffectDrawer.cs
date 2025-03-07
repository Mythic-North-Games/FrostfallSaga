using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomPropertyDrawer(typeof(AEffect), true)]
    public class AEffectDrawer : PropertyDrawer
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
            var currentY = position.y + EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Draw a title with the name of the effect class
                EditorGUI.LabelField(
                    new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                    "Effect Type: " + property.managedReferenceFullTypename.Split(".")[^1]
                );
                currentY += EditorGUIUtility.singleLineHeight * 2;
                expandedHeight += EditorGUIUtility.singleLineHeight * 2;

                // Draw properties specific to effect subclasses
                if (property.managedReferenceValue is ApplyStatusesEffect)
                {
                    BuildAndDrawReorderableList<AStatus>(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("StatusesToApply")
                    );
                }
                else if (property.managedReferenceValue is RemoveStatusesEffect)
                {
                    DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("RemovableStatusTypes")
                    );
                }
                else if (property.managedReferenceValue is HealEffect)
                {
                    DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("HealAmount")
                    );
                }
                else if (property.managedReferenceValue is PhysicalDamageEffect)
                {
                    DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("PhysicalDamageAmount")
                    );
                }
                else if (property.managedReferenceValue is MagicalDamageEffect)
                {
                    currentY = DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("MagicalDamageAmount")
                    );
                    DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("MagicalElement")
                    );
                }
                else if (property.managedReferenceValue is UpdateMutableStatEffect)
                {
                    currentY = DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("StatToUpdate")
                    );
                    currentY = DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("Amount")
                    );
                    DrawPropertyField(
                        new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight),
                        property.FindPropertyRelative("UsePercentage")
                    );
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Use expandedHeight computed during OnGUI for accurate inspector height
            return property.isExpanded ? expandedHeight : EditorGUIUtility.singleLineHeight;
        }

        private float DrawPropertyField(Rect fieldRect, SerializedProperty property)
        {
            // Check if the property is an array to handle height accordingly
            var propertyHeight = EditorGUI.GetPropertyHeight(property, true);
            EditorGUI.PropertyField(new Rect(fieldRect.x, fieldRect.y, fieldRect.width, propertyHeight), property,
                true);

            // Increment expandedHeight by the full height of the property (including arrays' full height)
            expandedHeight += propertyHeight + EditorGUIUtility.standardVerticalSpacing;

            // Return the updated Y position for the next field
            return fieldRect.y + propertyHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private float BuildAndDrawReorderableList<T>(Rect fieldRect, SerializedProperty property)
        {
            ReorderableList reorderableList = AbstractListEditorBuilder.BuildAbstractList<T>(
                property.serializedObject, property
            );

            // Draw the ReorderableList and compute its height
            var listHeight = reorderableList.GetHeight();
            reorderableList.DoList(new Rect(fieldRect.x, fieldRect.y, fieldRect.width, listHeight));

            // Adjust expandedHeight and return the new y position
            expandedHeight += listHeight + EditorGUIUtility.standardVerticalSpacing;
            return fieldRect.y + listHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}