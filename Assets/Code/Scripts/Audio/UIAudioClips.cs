using UnityEngine;

namespace FrostfallSaga.Audio
{
    [CreateAssetMenu(fileName = "UIAudioClips", menuName = "ScriptableObjects/Audio/UIAudioClips")]
    public class UIAudioClips : ScriptableObject
    {
        [field: SerializeField] public AudioClip ButtonHover { get; private set; }
        [field: SerializeField] public AudioClip ButtonClick { get; private set; }
        [field: SerializeField] public AudioClip FightStart { get; private set; }
        [field: SerializeField] public AudioClip FightWon { get; private set; }
        [field: SerializeField] public AudioClip FightLost { get; private set; }
    }
}