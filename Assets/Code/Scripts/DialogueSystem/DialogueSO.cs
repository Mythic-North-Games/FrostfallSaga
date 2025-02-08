using UnityEngine;
using FrostfallSaga.Utils.Trees;

namespace FrostfallSaga.DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogues/Dialogue", order = 0)]
    public class DialogueSO : ScriptableObject
    {
        [field: SerializeField] public TreeNode<DialogueLine> DialogueTreeRoot { get; private set; }

        public void SetRoot(TreeNode<DialogueLine> root)
        {
            DialogueTreeRoot = root;
        }

        public void PrintContent()
        {
            Debug.Log($"{DialogueTreeRoot.GetData().Title}");
            foreach(TreeNode<DialogueLine> child in DialogueTreeRoot.GetChildren())
            {
                Debug.Log($"{child.GetData().Title}");
            }
        }
    }
}