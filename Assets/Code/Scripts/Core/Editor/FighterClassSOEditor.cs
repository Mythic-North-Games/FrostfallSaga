using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils.DataStructures.GraphNode;

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
            _fighterClassSO.RebuildRuntimeGraph(); // Ensure graph is ready
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ability Graph Editor", EditorStyles.boldLabel);

            if (_fighterClassSO.AbilitiesGraphRoot == null)
            {
                if (GUILayout.Button("Create Root Node"))
                {
                    string newId = Guid.NewGuid().ToString();
                    var rootNode = new GraphNode<ABaseAbility>(newId, null);
                    _fighterClassSO.GetGraphMap()[newId] = rootNode;
                    _fighterClassSO.SaveRuntimeGraph();
                }
            }
            else
            {
                _showGraph = EditorGUILayout.Foldout(_showGraph, "Ability Graph");
                if (_showGraph)
                {
                    EditorGUI.indentLevel++;
                    DrawGraph(_fighterClassSO.AbilitiesGraphRoot);
                    EditorGUI.indentLevel--;
                }
            }

            if (_nodeToRemove != null)
            {
                RemoveNode(_nodeToRemove);
                _nodeToRemove = null;
                _fighterClassSO.SaveRuntimeGraph();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_fighterClassSO);
            }
        }

        private void DrawGraph(GraphNode<ABaseAbility> node)
        {
            if (node == null)
                return;

            string label = (node.Data == null) ? "Untitled Node" : node.Data.Name;

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
                    _fighterClassSO.SaveRuntimeGraph();
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

            string newId = Guid.NewGuid().ToString();
            var newNode = new GraphNode<ABaseAbility>(newId, null);

            parent.Children.Add(newNode);
            newNode.Parents.Add(parent);

            _fighterClassSO.GetGraphMap()[newId] = newNode;
            _nodeFoldouts[newNode] = true;
            _fighterClassSO.SaveRuntimeGraph();
        }

        private void RemoveNode(GraphNode<ABaseAbility> node)
        {
            if (node == null || node.Parents == null) return;

            foreach (var parent in node.Parents)
                parent.Children.Remove(node);

            foreach (var child in node.Children)
                child.Parents.Remove(node);

            _fighterClassSO.GetGraphMap().Remove(node.ID);
            _nodeFoldouts.Remove(node);
        }
    }
}