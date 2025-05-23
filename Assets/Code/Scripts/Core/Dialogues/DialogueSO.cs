using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils.DataStructures.TreeNode;

namespace FrostfallSaga.Core.Dialogues
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogues/Dialogue", order = 0)]
    public class DialogueSO : ScriptableObject
    {
        [field: SerializeField] public Sprite DialogueBackground { get; private set; }

        /////////////////////
        /// Dialogue tree ///
        /////////////////////
        public TreeNode<DialogueLine> DialogueTreeRoot => _runtimeTree;
        [SerializeField] private List<TreeNodeDTO<DialogueLine>> _serializedTree;
        private TreeNode<DialogueLine> _runtimeTree;

        private void OnEnable()
        {
            _runtimeTree = TreeNodeSerializer.DeserializeTree(_serializedTree);
        }

#if UNITY_EDITOR
        public void SetRoot(TreeNode<DialogueLine> root)
        {
            _runtimeTree = root;
            SaveTree();
        }

        public void SaveTree()
        {
            _serializedTree = TreeNodeSerializer.SerializeTree(_runtimeTree);
        }
#endif

        public void PrintContent()
        {
            if (_runtimeTree?.Data == null) return;

            Debug.Log($"{_runtimeTree.Data.Title}");
            foreach (TreeNode<DialogueLine> child in _runtimeTree.Children)
                Debug.Log($"{child.Data.Title}");
        }
    }
}
