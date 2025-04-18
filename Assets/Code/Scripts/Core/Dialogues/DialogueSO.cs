using FrostfallSaga.Utils.Trees;
using UnityEngine;

namespace FrostfallSaga.Core.Dialogues
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogues/Dialogue", order = 0)]
    public class DialogueSO : ScriptableObject
    {
        [field: SerializeField] public TreeNode<DialogueLine> DialogueTreeRoot { get; private set; }
        [field: SerializeField] public Sprite DialogueBackground { get; private set; }

        public void SetRoot(TreeNode<DialogueLine> root)
        {
            DialogueTreeRoot = root;
        }

        public void PrintContent()
        {
            Debug.Log($"{DialogueTreeRoot.Data.Title}");
            foreach (TreeNode<DialogueLine> child in DialogueTreeRoot.Children)
                Debug.Log($"{child.Data.Title}");
        }
    }
}