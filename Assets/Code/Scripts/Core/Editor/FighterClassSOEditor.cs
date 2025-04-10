using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils.Trees;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.Core
{
    [CustomEditor(typeof(FighterClassSO))]
    public class FighterClassSOEditor : Editor
    {
        private readonly Dictionary<TreeNode<ABaseAbility>, bool> _nodeFoldouts = new();
        private FighterClassSO _fighterClassSO;
        private TreeNode<ABaseAbility> _nodeToRemove = null;
        private bool _showTree = true;
        public Action onDialogueChanged;

        private void OnEnable()
        {
            if (target == null)
                return;
            _fighterClassSO = (FighterClassSO)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ability Tree Editor", EditorStyles.boldLabel);

            if (_fighterClassSO.AbilitiesTreeModel == null)
            {
                if (GUILayout.Button("Create Root Node"))
                {
                    _fighterClassSO.SetRoot(new TreeNode<ABaseAbility>(null));
                }
            }
            else
            {
                _showTree = EditorGUILayout.Foldout(_showTree, "Ability Tree");

                if (_showTree)
                {
                    EditorGUI.indentLevel++;
                    DrawTree(_fighterClassSO.AbilitiesTreeModel, null);
                    EditorGUI.indentLevel--;
                }
            }

            // Suppression du n�ud s�lectionn�
            if (_nodeToRemove != null)
            {
                RemoveNode(_nodeToRemove);
                _nodeToRemove = null;
                GUI.changed = true; // Force la mise � jour de l'UI
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawTree(TreeNode<ABaseAbility> node, TreeNode<ABaseAbility> parent)
        {
            if (node == null)
                return;

            var data = node.GetData();
            _nodeFoldouts.TryAdd(node, true);

            // D�but du bloc pour le n�ud
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            _nodeFoldouts[node] = EditorGUILayout.Foldout(
                _nodeFoldouts[node],
                data == null ? "Untitled Node" : data.Name,
                true
            );

            if (parent != null && GUILayout.Button("Remove", GUILayout.MaxWidth(70)))
            {
                _nodeToRemove = node;
            }

            EditorGUILayout.EndHorizontal();

            // Si le n�ud est ouvert
            if (_nodeFoldouts[node])
            {
                // S�lection de l'Ability
                ABaseAbility newAbility = (ABaseAbility)EditorGUILayout.ObjectField(
                    "Ability", data, typeof(ABaseAbility), false
                );
                if (newAbility != data)
                {
                    node.SetData(newAbility);
                    EditorUtility.SetDirty(_fighterClassSO);
                }

                // Affichage des enfants
                EditorGUILayout.Space();
                foreach (TreeNode<ABaseAbility> child in new List<TreeNode<ABaseAbility>>(node.GetChildren()))
                {
                    EditorGUI.indentLevel++;
                    DrawTree(child, node);
                    EditorGUI.indentLevel--;
                }

                // ?? Bouton "Add Ability" **� l'int�rieur** du n�ud
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20 * EditorGUI.indentLevel); // Indentation correcte
                if (GUILayout.Button("+ Add Ability", GUILayout.MaxWidth(150)))
                {
                    node.GetChildren().Add(new TreeNode<ABaseAbility>(null));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void RemoveNode(TreeNode<ABaseAbility> node)
        {
            Queue<TreeNode<ABaseAbility>> queue = new();
            queue.Enqueue(_fighterClassSO.AbilitiesTreeModel);

            while (queue.Count > 0)
            {
                TreeNode<ABaseAbility> current = queue.Dequeue();

                if (current.GetChildren().Contains(node))
                {
                    current.GetChildren().Remove(node);
                    _nodeFoldouts.Remove(node); // Nettoyage du dictionnaire
                    return;
                }

                foreach (var child in current.GetChildren())
                {
                    queue.Enqueue(child);
                }
            }
        }
    }
}