using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.DataStructures.TreeNode;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    [CustomEditor(typeof(DialogueSO))]
    public class DialogueSOEditor : Editor
    {
        public Action onDialogueChanged;

        private DialogueSO _dialogueSO;
        private bool _showTree = true;
        private readonly Dictionary<TreeNode<DialogueLine>, bool> _nodeFoldouts = new();

        private void OnEnable()
        {
            _dialogueSO = (DialogueSO)target;

            if (_dialogueSO.DialogueTreeRoot == null)
            {
                string rootId = Guid.NewGuid().ToString();
                var rootNode = new TreeNode<DialogueLine>(rootId, new DialogueLine("Dialogue start", "Starter line", null, false));
                _dialogueSO.SetRoot(rootNode);
                EditorUtility.SetDirty(_dialogueSO);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dialogue Tree Editor", EditorStyles.boldLabel);

            if (_dialogueSO.DialogueTreeRoot?.Data == null) return;

            _showTree = EditorGUILayout.Foldout(_showTree, "Dialogue Tree");
            if (_showTree)
            {
                EditorGUI.indentLevel++;
                DrawTree(_dialogueSO.DialogueTreeRoot, null);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawTree(TreeNode<DialogueLine> node, TreeNode<DialogueLine> parent)
        {
            if (node == null) return;

            DialogueLine data = node.Data;
            string label = string.IsNullOrWhiteSpace(data?.Title) ? "Untitled Node" : data.Title;
            bool isRoot = parent == null;

            if (!_nodeFoldouts.ContainsKey(node)) _nodeFoldouts[node] = true;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            _nodeFoldouts[node] = EditorGUILayout.Foldout(_nodeFoldouts[node], label, true);

            if (!isRoot && GUILayout.Button("Remove Node", GUILayout.MaxWidth(100)))
            {
                int index = parent.Children.IndexOf(node);
                parent.RemoveChild(node);

                if (index >= 0 && parent.Data.Answers != null && index < parent.Data.Answers.Length)
                {
                    List<string> answersList = new(parent.Data.Answers);
                    answersList.RemoveAt(index);
                    parent.Data.SetAnswers(answersList.ToArray());
                }

                _dialogueSO.SaveTree();
                EditorUtility.SetDirty(_dialogueSO);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.EndHorizontal();

            if (_nodeFoldouts[node])
            {
                string newTitle = EditorGUILayout.TextField("Title", data.Title);
                if (newTitle != data.Title) { data.SetTitle(newTitle); MarkChanged(); }

                string newRichText = EditorGUILayout.TextField("Rich Text", data.RichText);
                if (newRichText != data.RichText) { data.SetRichText(newRichText); MarkChanged(); }

                DialogueParticipantSO newSpeaker = (DialogueParticipantSO)EditorGUILayout.ObjectField("Speaker", data.Speaker, typeof(DialogueParticipantSO), false);
                if (newSpeaker != data.Speaker) { data.SetSpeaker(newSpeaker); MarkChanged(); }

                bool newIsRight = EditorGUILayout.Toggle("Is Speaker on Right?", data.IsRight);
                if (newIsRight != data.IsRight) { data.SetIsRight(newIsRight); MarkChanged(); }

                AQuestSO newQuest = (AQuestSO)EditorGUILayout.ObjectField("Quest", data.Quest, typeof(AQuestSO), false);
                if (newQuest != data.Quest) { data.SetQuest(newQuest); MarkChanged(); }

                EditorGUILayout.Space();
                DrawAnswersAndChildren(node);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawAnswersAndChildren(TreeNode<DialogueLine> node)
        {
            DialogueLine data = node.Data;

            if (data.Answers != null && data.Answers.Length > 0)
            {
                EditorGUILayout.LabelField("Answers:", EditorStyles.boldLabel);
                for (int i = 0; i < data.Answers.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    string newAnswer = EditorGUILayout.TextField($"Answer {i + 1}", data.Answers[i]);

                    if (newAnswer != data.Answers[i])
                    {
                        data.Answers[i] = newAnswer;
                        if (node.Children != null && i < node.Children.Count)
                            node.Children[i].Data.SetTitle($"{newAnswer} answer");
                        MarkChanged();
                    }

                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        RemoveAnswerAndChild(node, i);
                        EditorGUILayout.EndHorizontal();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (node.Children != null && i < node.Children.Count)
                    {
                        EditorGUI.indentLevel++;
                        DrawTree(node.Children[i], node);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Previous dialogue line continuation:", EditorStyles.boldLabel);

                if (node.Children == null || node.Children.Count == 0)
                {
                    if (GUILayout.Button("Add line continuation"))
                    {
                        node.Children ??= new List<TreeNode<DialogueLine>>();
                        string childId = Guid.NewGuid().ToString();
                        node.AddChild(new TreeNode<DialogueLine>(childId, new DialogueLine($"{data.Title} continuation", $"Continuation of {data.Title}", null, false)));
                        MarkChanged();
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;
                    DrawTree(node.Children[0], node);
                    EditorGUI.indentLevel--;
                }
            }

            if (node.Children.Count == 1 && data.Answers == null) return;

            if (GUILayout.Button("Add answer"))
                AddAnswerAndChild(node, "New answer");
        }

        private void AddAnswerAndChild(TreeNode<DialogueLine> parentNode, string answerText)
        {
            DialogueLine data = parentNode.Data;
            List<string> answersList = data.Answers != null ? new List<string>(data.Answers) : new List<string>();
            answersList.Add(answerText);
            data.SetAnswers(answersList.ToArray());

            string childId = Guid.NewGuid().ToString();
            parentNode.AddChild(new TreeNode<DialogueLine>(childId, new DialogueLine($"{answerText} answer", "New dialogue line", null, false)));
            MarkChanged();
        }

        private void RemoveAnswerAndChild(TreeNode<DialogueLine> parentNode, int index)
        {
            DialogueLine data = parentNode.Data;
            if (data.Answers != null && index < data.Answers.Length)
            {
                List<string> answersList = new(data.Answers);
                answersList.RemoveAt(index);
                data.SetAnswers(answersList.ToArray());
            }

            if (parentNode.Children != null && index < parentNode.Children.Count)
                parentNode.Children.RemoveAt(index);

            MarkChanged();
        }

        private void MarkChanged()
        {
            _dialogueSO.SaveTree();
            EditorUtility.SetDirty(_dialogueSO);
        }
    }
}
