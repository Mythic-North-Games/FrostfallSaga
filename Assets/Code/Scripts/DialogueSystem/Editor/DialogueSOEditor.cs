using UnityEditor;
using UnityEngine;
using FrostfallSaga.DialogueSystem;
using FrostfallSaga.Utils.Trees;
using System.Collections.Generic;

namespace FrostfallSaga.FFSEditor.DialogueSystem
{
    /// <summary>
    /// To quickly visualize and edit a dialogue scriptable object.
    /// </summary>
    [CustomEditor(typeof(DialogueSO))]
    public class DialogueSOEditor : Editor
    {
        private DialogueSO _dialogueSO;
        private bool _showTree = true;
        private Dictionary<TreeNode<DialogueLine>, bool> _nodeFoldouts = new();  // Track foldouts per node

        private void OnEnable()
        {
            _dialogueSO = (DialogueSO)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dialogue Tree Editor", EditorStyles.boldLabel);

            if (_dialogueSO.DialogueTreeRoot == null)
            {
                if (GUILayout.Button("Create Root Node"))
                {
                    _dialogueSO.SetRoot(new TreeNode<DialogueLine>(
                        new DialogueLine("Dialogue start", "Starter line", null, false)
                    ));
                    EditorUtility.SetDirty(_dialogueSO);
                }
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
                EditorUtility.SetDirty(_dialogueSO);
            }
        }

        private void DrawTree(TreeNode<DialogueLine> node, TreeNode<DialogueLine> parent)
        {
            if (node == null) return;

            var data = node.GetData();

            // Initialize foldout state if not already tracked
            if (!_nodeFoldouts.ContainsKey(node))
                _nodeFoldouts[node] = true;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            // Foldout for node content
            _nodeFoldouts[node] = EditorGUILayout.Foldout(_nodeFoldouts[node], string.IsNullOrEmpty(data.Title) ? "Untitled Node" : data.Title, true);

            // Remove Node Button (if not root)
            if (parent != null)
            {
                if (GUILayout.Button("Remove Node", GUILayout.MaxWidth(100)))
                {
                    int index = parent.GetChildren().IndexOf(node);
                    parent.RemoveChild(node);

                    // Also remove corresponding answer from the parent node
                    if (index >= 0 && parent.GetData().Answers != null && index < parent.GetData().Answers.Length)
                    {
                        var answersList = new List<string>(parent.GetData().Answers);
                        answersList.RemoveAt(index);
                        parent.GetData().SetAnswers(answersList.ToArray());
                    }

                    EditorUtility.SetDirty(_dialogueSO);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Display node details if foldout is open
            if (_nodeFoldouts[node])
            {
                data.SetTitle(EditorGUILayout.TextField("Title", data.Title));
                data.SetRichText(EditorGUILayout.TextField("Rich Text", data.RichText));
                data.SetSpeaker((DialogueParticipantSO)EditorGUILayout.ObjectField("Speaker", data.Speaker, typeof(DialogueParticipantSO), false));
                data.SetIsRight(EditorGUILayout.Toggle("Is Speaker on Right?", data.IsRight));

                EditorGUILayout.Space();

                // Display and manage answers
                DrawAnswersAndChildren(node);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void DrawAnswersAndChildren(TreeNode<DialogueLine> node)
        {
            var data = node.GetData();

            // Handle Answers
            if (data.Answers != null && data.Answers.Length > 0)
            {
                EditorGUILayout.LabelField("Answers:", EditorStyles.boldLabel);

                for (int i = 0; i < data.Answers.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Update Answer Text
                    string newAnswer = EditorGUILayout.TextField($"Answer {i + 1}", data.Answers[i]);

                    // Update child node title if answer text changes
                    if (newAnswer != data.Answers[i])
                    {
                        data.Answers[i] = newAnswer;

                        // Ensure child node title updates accordingly
                        if (node.GetChildren() != null && i < node.GetChildren().Count)
                        {
                            var childNode = node.GetChildren()[i];
                            childNode.GetData().SetTitle($"{newAnswer} answer");
                        }
                    }

                    // Remove Answer Button
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        RemoveAnswerAndChild(node, i);
                        EditorGUILayout.EndHorizontal();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    // Display the corresponding child node directly under the answer
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
                // If no answers, allow a single child node for continuation
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Previous dialogue line contiuation:", EditorStyles.boldLabel);

                if (node.GetChildren() == null || node.GetChildren().Count == 0)
                {
                    if (GUILayout.Button("Add line continuation"))
                    {
                        if (node.GetChildren() == null)
                            node.SetChildren(new List<TreeNode<DialogueLine>>());

                        node.GetChildren().Add(new TreeNode<DialogueLine>(
                            new DialogueLine($"{node.GetData().Title} line continuation", $"Continuation of {node.GetData().Title} line", null, false)
                        ));

                        EditorUtility.SetDirty(_dialogueSO);
                    }
                }
                else
                {
                    // Display the single child node if it exists
                    EditorGUI.indentLevel++;
                    DrawTree(node.GetChildren()[0], node);
                    EditorGUI.indentLevel--;
                }
            }

            // Add Answer Button if no direct dialogue continuation
            if (node.GetChildren().Count == 1 && node.GetData().Answers == null) return;
            if (GUILayout.Button("Add answer"))
            {
                AddAnswerAndChild(node, "New answer");
            }
        }

        private void AddAnswerAndChild(TreeNode<DialogueLine> parentNode, string answerText)
        {
            var data = parentNode.GetData();
            List<string> answersList = data.Answers != null ? new List<string>(data.Answers) : new List<string>();

            answersList.Add(answerText);
            data.SetAnswers(answersList.ToArray());

            // Add corresponding child node with title based on the answer
            if (parentNode.GetChildren() == null)
                parentNode.SetChildren(new List<TreeNode<DialogueLine>>());

            parentNode.GetChildren().Add(new TreeNode<DialogueLine>(
                new DialogueLine($"{answerText} answer", "New dialogue line", null, false)
            ));

            EditorUtility.SetDirty(_dialogueSO);
        }

        private void RemoveAnswerAndChild(TreeNode<DialogueLine> parentNode, int index)
        {
            var data = parentNode.GetData();

            if (data.Answers != null && index < data.Answers.Length)
            {
                List<string> answersList = new(data.Answers);
                answersList.RemoveAt(index);
                data.SetAnswers(answersList.ToArray());
            }

            if (parentNode.GetChildren() != null && index < parentNode.GetChildren().Count)
            {
                parentNode.GetChildren().RemoveAt(index);
            }

            EditorUtility.SetDirty(_dialogueSO);
        }
    }
}