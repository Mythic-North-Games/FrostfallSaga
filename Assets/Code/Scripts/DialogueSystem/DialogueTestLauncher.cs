using FrostfallSaga.Core.Dialogues;
using UnityEngine;

namespace FrostfallSaga.DialogueSystem
{
    /// <summary>
    ///     A simple launcher to test the dialogue system.
    /// </summary>
    public class DialogueTestLauncher : MonoBehaviour
    {
        [SerializeField] private DialogueSO _dialogueSO;
        [SerializeField] private DialogueUIProcessor _dialogueUIProcessor;

        private void Start()
        {
            _dialogueUIProcessor.ProcessDialogue(_dialogueSO);
        }
    }
}