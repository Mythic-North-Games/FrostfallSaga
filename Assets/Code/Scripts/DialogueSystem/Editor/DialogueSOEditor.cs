using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Trees;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    [CustomEditor(typeof(DialogueSO))]
    public class DialogueSOEditor : Editor
    {
        private readonly Dictionary<TreeNode<DialogueLine>, bool> _nodeFoldouts = new();
        private DialogueSO _dialogueSO;
        private bool _showTree = true;
        public Action onDialogueChanged;

        private void OnEnable()
        {
            if (target == null) return;
            _dialogueSO = (DialogueSO)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dialogue Tree Editor", EditorStyles.boldLabel);

            if (_dialogueSO.DialogueTreeRoot == null || _dialogueSO.DialogueTreeRoot.GetData() == null)
            {
                if (GUILayout.Button("Create Root Node"))
                    _dialogueSO.SetRoot(new TreeNode<DialogueLine>(
                        new DialogueLine("Dialogue start", "Starter line", null, false)
                    ));
            }
            else
            {
                _showTree = EditorGUILayout.Foldout(_showTree, "Dialogue Tree");

                if (_showTree)
                {
                    EditorGUI.indentLevel++;
                    DrawTree(_dialogueSO.DialogueTreeRoot, null);
                    EditorGUI.indentLevel--;
                }
            }

            if (GUI.changed)
            {
                onDialogueChanged?.Invoke();
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawTree(TreeNode<DialogueLine> node, TreeNode<DialogueLine> parent)
        {
            if (node == null) return;

            DialogueLine data = node.GetData();

            if (!_nodeFoldouts.ContainsKey(node))
                _nodeFoldouts[node] = true;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            _nodeFoldouts[node] = EditorGUILayout.Foldout(_nodeFoldouts[node],
                string.IsNullOrEmpty(data.Title) ? "Untitled Node" : data.Title, true);

            if (parent != null)
                if (GUILayout.Button("Remove Node", GUILayout.MaxWidth(100)))
                {
                    int index = parent.GetChildren().IndexOf(node);
                    parent.RemoveChild(node);

                    if (index >= 0 && parent.GetData().Answers != null && index < parent.GetData().Answers.Length)
                    {
                        List<string> answersList = new(parent.GetData().Answers);
                        answersList.RemoveAt(index);
                        parent.GetData().SetAnswers(answersList.ToArray());
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

            EditorGUILayout.EndHorizontal();

            if (_nodeFoldouts[node])
            {
                string newTitle = EditorGUILayout.TextField("Title", data.Title);
                if (newTitle != data.Title)
                {
                    data.SetTitle(newTitle);
                    EditorUtility.SetDirty(_dialogueSO);
                }

                string newRichText = EditorGUILayout.TextField("Rich Text", data.RichText);
                if (newRichText != data.RichText)
                {
                    data.SetRichText(newRichText);
                    EditorUtility.SetDirty(_dialogueSO);
                }

                DialogueParticipantSO newSpeaker = (DialogueParticipantSO)EditorGUILayout.ObjectField(
                    "Speaker", data.Speaker, typeof(DialogueParticipantSO), false
                );
                if (newSpeaker != data.Speaker)
                {
                    data.SetSpeaker(newSpeaker);
                    EditorUtility.SetDirty(_dialogueSO);
                }

                bool newIsRight = EditorGUILayout.Toggle("Is Speaker on Right?", data.IsRight);
                if (newIsRight != data.IsRight)
                {
                    data.SetIsRight(newIsRight);
                    EditorUtility.SetDirty(_dialogueSO);
                }

                AQuestSO newQuest = (AQuestSO)EditorGUILayout.ObjectField(
                    "Quest", data.Quest, typeof(AQuestSO), false
                );
                if (newQuest != data.Quest)
                {
                    data.SetQuest(newQuest);
                    EditorUtility.SetDirty(_dialogueSO);
                }

                EditorGUILayout.Space();
                DrawAnswersAndChildren(node);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawAnswersAndChildren(TreeNode<DialogueLine> node)
        {
            DialogueLine data = node.GetData();

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

                        if (node.GetChildren() != null && i < node.GetChildren().Count)
                        {
                            TreeNode<DialogueLine> childNode = node.GetChildren()[i];
                            childNode.GetData().SetTitle($"{newAnswer} answer");
                        }
                    }

                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        RemoveAnswerAndChild(node, i);
                        EditorGUILayout.EndHorizontal();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (node.GetChildren() != null && i < node.GetChildren().Count)
                    {
                        EditorGUI.indentLevel++;
                        DrawTree(node.GetChildren()[i], node);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Previous dialogue line continuation:", EditorStyles.boldLabel);

                if (node.GetChildren() == null || node.GetChildren().Count == 0)
                {
                    if (GUILayout.Button("Add line continuation"))
                    {
                        if (node.GetChildren() == null)
                            node.SetChildren(new List<TreeNode<DialogueLine>>());

                        node.GetChildren().Add(new TreeNode<DialogueLine>(
                            new DialogueLine($"{node.GetData().Title} line continuation",
                                $"Continuation of {node.GetData().Title} line", null, false)
                        ));
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;
                    DrawTree(node.GetChildren()[0], node);
                    EditorGUI.indentLevel--;
                }
            }

            if (node.GetChildren().Count == 1 && node.GetData().Answers == null) return;

            if (GUILayout.Button("Add answer")) AddAnswerAndChild(node, "New answer");
        }


        private void AddAnswerAndChild(TreeNode<DialogueLine> parentNode, string answerText)
        {
            DialogueLine data = parentNode.GetData();
            List<string> answersList = data.Answers != null ? new List<string>(data.Answers) : new List<string>();

            answersList.Add(answerText);
            data.SetAnswers(answersList.ToArray());

            if (parentNode.GetChildren() == null)
                parentNode.SetChildren(new List<TreeNode<DialogueLine>>());

            parentNode.GetChildren().Add(new TreeNode<DialogueLine>(
                new DialogueLine($"{answerText} answer", "New dialogue line", null, false)
            ));

            EditorUtility.SetDirty(_dialogueSO);
        }

        private void RemoveAnswerAndChild(TreeNode<DialogueLine> parentNode, int index)
        {
            DialogueLine data = parentNode.GetData();

            if (data.Answers != null && index < data.Answers.Length)
            {
                List<string> answersList = new(data.Answers);
                answersList.RemoveAt(index);
                data.SetAnswers(answersList.ToArray());
            }

            if (parentNode.GetChildren() != null && index < parentNode.GetChildren().Count)
                parentNode.GetChildren().RemoveAt(index);

            EditorUtility.SetDirty(_dialogueSO);
        }
    }
}