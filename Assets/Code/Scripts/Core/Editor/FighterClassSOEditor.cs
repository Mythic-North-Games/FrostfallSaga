using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils.Trees;

namespace FrostfallSaga.Core
{
    [CustomEditor(typeof(FighterClassSO))]
    public class FighterClassSOEditor : Editor
    {
        private FighterClassSO _fighterClassSO;
        private bool _showGraph = true;
        private readonly Dictionary<GraphNode<ABaseAbility>, bool> _nodeFoldouts = new();
        private GraphNode<ABaseAbility> _nodeToRemove;

        private void OnEnable()
        {
            if (target == null) return;
            _fighterClassSO = (FighterClassSO)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ability Graph Editor", EditorStyles.boldLabel);

            if (_fighterClassSO.AbilitiesGraphModel == null)
            {
                if (GUILayout.Button("Create Root Node"))
                {
                    _fighterClassSO.SetGraphRoot(new GraphNode<ABaseAbility> { Data = null });
                }
            }
            else
            {
                _showGraph = EditorGUILayout.Foldout(_showGraph, "Ability Graph");
                if (_showGraph)
                {
                    EditorGUI.indentLevel++;
                    DrawGraph(_fighterClassSO.AbilitiesGraphModel);
                    EditorGUI.indentLevel--;
                }
            }

            if (_nodeToRemove != null)
            {
                RemoveNode(_nodeToRemove);
                _nodeToRemove = null;
            }
        }

        private void DrawGraph(GraphNode<ABaseAbility> node)
        {
            if (node == null)
                return;

            string label = (node.Data == null) ? "Untitled Node" : node.Data.Name;
            bool isRoot = node.Parents == null || node.Parents.Count == 0;

            if (!_nodeFoldouts.ContainsKey(node)) _nodeFoldouts[node] = true;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            _nodeFoldouts[node] = EditorGUILayout.Foldout(_nodeFoldouts[node], label, true);
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(70)))
            {
                _nodeToRemove = node;                
            }

            EditorGUILayout.EndHorizontal();

            if (_nodeFoldouts.ContainsKey(node) && _nodeFoldouts[node])
            {
                ABaseAbility newAbility = (ABaseAbility)EditorGUILayout.ObjectField("Ability", node.Data, typeof(ABaseAbility), false);
                if (newAbility != node.Data)
                {
                    node.Data = newAbility;
                    EditorUtility.SetDirty(_fighterClassSO);
                }

                EditorGUILayout.Space();
                foreach (var child in node.Children)
                {
                    EditorGUI.indentLevel++;
                    DrawGraph(child);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20 * EditorGUI.indentLevel);
                if (GUILayout.Button("+ Add Ability", GUILayout.MaxWidth(150)))
                {
                    AddNode(node);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void AddNode(GraphNode<ABaseAbility> parent)
        {
            if (parent == null) return;

            GraphNode<ABaseAbility> newNode = new() { Data = null };
            parent.Children.Add(newNode);
            newNode.Parents.Add(parent);
            _nodeFoldouts[newNode] = true;
            EditorUtility.SetDirty(_fighterClassSO);
        }

        private void RemoveNode(GraphNode<ABaseAbility> node)
        {
            if (node == null) return;

            foreach (GraphNode<ABaseAbility> parent in node.Parents)
            {
                if (parent.Children.Contains(node)) parent.Children.Remove(node);
            }
            _nodeFoldouts.Remove(node);
            EditorUtility.SetDirty(_fighterClassSO);
        }
    }
}