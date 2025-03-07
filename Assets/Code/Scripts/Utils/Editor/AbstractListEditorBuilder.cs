using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FrostfallSaga.Utils.Editor
{
    public static class AbstractListEditorBuilder
    {
        public static ReorderableList BuildAbstractList<T>(
            SerializedObject serializedObject,
            SerializedProperty abstractListProperty
        )
        {
            return new ReorderableList(serializedObject, abstractListProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, abstractListProperty.name); },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = abstractListProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element, new GUIContent($"{abstractListProperty.name} {index + 1}"),
                        true);
                },

                onAddDropdownCallback = (rect, list) =>
                {
                    GenericMenu menu = new();
                    List<Type> alterationTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes())
                        .Where(type => type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                        .ToList();

                    foreach (Type type in alterationTypes)
                        menu.AddItem(new GUIContent(type.Name), false, () =>
                        {
                            var newIndex = abstractListProperty.arraySize;
                            abstractListProperty.InsertArrayElementAtIndex(newIndex);

                            SerializedProperty newElement = abstractListProperty.GetArrayElementAtIndex(newIndex);
                            newElement.managedReferenceValue = (T)Activator.CreateInstance(type);
                            serializedObject.ApplyModifiedProperties();
                        });

                    menu.ShowAsContext();
                },

                onRemoveCallback = list =>
                {
                    if (list.index >= 0 && list.index < abstractListProperty.arraySize)
                        abstractListProperty.DeleteArrayElementAtIndex(list.index);
                },

                elementHeightCallback = index =>
                {
                    SerializedProperty element = abstractListProperty.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            };
        }
    }
}