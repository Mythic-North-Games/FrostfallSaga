using System.Collections.Generic;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Trees;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Quests
{
    [CustomEditor(typeof(StepsQuestSO))]
    public class StepsQuestSOEditor : Editor
    {
        private StepsQuestSO quest;
        private bool showTree = true;

        private void OnEnable()
        {
            quest = (StepsQuestSO)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.LabelField("Quest Steps", EditorStyles.boldLabel);
            showTree = EditorGUILayout.Foldout(showTree, "Quest Steps Tree");
            if (showTree) DrawQuestStepTree(quest.Steps, 0, new List<int>());

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawQuestStepTree(TreeNode<QuestStep> node, int indentLevel, List<int> currentPath)
        {
            if (node == null)
            {
                node = new TreeNode<QuestStep>(new QuestStep("First step", "Lorem ipsum dolor sit amet", null));
                SyncPossibleQuestEndings();
            }

            if (node.Data == null)
            {
                node.Data = new QuestStep("First step", "Lorem ipsum dolor sit amet", null);
                SyncPossibleQuestEndings();
            }

            QuestStep data = node.Data;
            EditorGUI.indentLevel = indentLevel;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Step: " + data.Title);
            data.SetTitle(EditorGUILayout.TextField("Title", data.Title));
            data.SetDescription(EditorGUILayout.TextField("Description", data.Description));
            data.SetActions((QuestStepActionsSO)EditorGUILayout.ObjectField("Actions", data.Actions,
                typeof(QuestStepActionsSO), false));

            if (GUILayout.Button("Add Child Step"))
            {
                Undo.RecordObject(quest, "Add Child Step");
                TreeNode<QuestStep> newChild = new(new QuestStep("New Step", "Description", null));
                node.AddChild(newChild);
                EditorUtility.SetDirty(quest);

                // Sync possible endings
                SyncPossibleQuestEndings();
            }

            if (node.Children == null) return;

            for (int i = 0; i < node.Children.Count; i++)
            {
                List<int> childPath = new(currentPath) { i };
                DrawQuestStepTree(node.Children[i], indentLevel + 1, childPath);
            }

            if (indentLevel > 0 && GUILayout.Button("Remove Step"))
            {
                Undo.RecordObject(quest, "Remove Step");
                node.Parent.RemoveChild(node);
                EditorUtility.SetDirty(quest);

                // Sync possible endings
                SyncPossibleQuestEndings();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private void SyncPossibleQuestEndings()
        {
            // Generate new list of endings based on the tree structure
            List<SElementToValue<int[], QuestEnding>> newPossibleEndings = new();
            List<int> currentPath = new() { 0 };
            GeneratePossibleEndings(quest.Steps, currentPath, newPossibleEndings);

            // Assign the new list to the quest object
            quest.SetPossibleQuestEndings(newPossibleEndings.ToArray());

            // Mark object as dirty so the editor updates
            EditorUtility.SetDirty(quest);
        }

        private void GeneratePossibleEndings(TreeNode<QuestStep> node, List<int> currentPath,
            List<SElementToValue<int[], QuestEnding>> endingsList)
        {
            if (node.Children == null)
            {
                endingsList.Add(new SElementToValue<int[], QuestEnding>(currentPath.ToArray(), null));
                return;
            }

            // If it's a leaf node, add the current path to the list
            if (node.Children.Count == 0)
            {
                endingsList.Add(new SElementToValue<int[], QuestEnding>(currentPath.ToArray(), null));
                return;
            }

            // Otherwise, recursively process children
            for (int i = 0; i < node.Children.Count; i++)
            {
                List<int> childPath = new(currentPath) { i };
                GeneratePossibleEndings(node.Children[i], childPath, endingsList);
            }
        }
    }
}