using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.DataStructures.TreeNode;

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
            if (quest.Steps == null)
            {
                string rootId = System.Guid.NewGuid().ToString();
                quest.SetSteps(new TreeNode<QuestStep>(rootId, new QuestStep("First Step", "Description", null)));
            }
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
            if (node == null || node.Data == null) return;

            QuestStep data = node.Data;
            EditorGUI.indentLevel = indentLevel;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Step: " + data.Title);
            data.SetTitle(EditorGUILayout.TextField("Title", data.Title));
            data.SetDescription(EditorGUILayout.TextField("Description", data.Description));
            data.SetActions((QuestStepActionsSO)EditorGUILayout.ObjectField("Actions", data.Actions, typeof(QuestStepActionsSO), false));

            if (GUILayout.Button("Add Child Step"))
            {
                Undo.RecordObject(quest, "Add Child Step");
                var childId = System.Guid.NewGuid().ToString();
                TreeNode<QuestStep> newChild = new(childId, new QuestStep("New Step", "Description", null));
                node.AddChild(newChild);
                quest.SaveSteps();
                EditorUtility.SetDirty(quest);
                SyncPossibleQuestEndings();
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                List<int> childPath = new(currentPath) { i };
                DrawQuestStepTree(node.Children[i], indentLevel + 1, childPath);
            }

            if (indentLevel > 0 && GUILayout.Button("Remove Step"))
            {
                Undo.RecordObject(quest, "Remove Step");
                node.Parent.RemoveChild(node);
                quest.SaveSteps();
                EditorUtility.SetDirty(quest);
                SyncPossibleQuestEndings();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private void SyncPossibleQuestEndings()
        {
            Dictionary<int[], QuestEnding> newPossibleEndings = new();
            List<int> currentPath = new() { 0 };
            GeneratePossibleEndings(quest.Steps, currentPath, newPossibleEndings);
            quest.SetPossibleQuestEndings(newPossibleEndings);
            EditorUtility.SetDirty(quest);
        }

        private void GeneratePossibleEndings(TreeNode<QuestStep> node, List<int> currentPath, Dictionary<int[], QuestEnding> endingsList)
        {
            if (node.Children == null || node.Children.Count == 0)
            {
                endingsList.Add(currentPath.ToArray(), null);
                return;
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                List<int> childPath = new(currentPath) { i };
                GeneratePossibleEndings(node.Children[i], childPath, endingsList);
            }
        }
    }
}
