using UnityEngine;

namespace FrostfallSaga.Core.Dialogues
{
    [CreateAssetMenu(fileName = "DialogueParticipant", menuName = "ScriptableObjects/Dialogues/DialogueParticipant",
        order = 0)]
    public class DialogueParticipantSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}