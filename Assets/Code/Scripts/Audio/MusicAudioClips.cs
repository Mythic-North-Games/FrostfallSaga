using UnityEngine;

namespace FrostfallSaga.Audio
{
    [CreateAssetMenu(fileName = "MusicAudioClips", menuName = "ScriptableObjects/Audio/MusicAudioClips")]
    public class MusicAudioClips : ScriptableObject
    {
        [field: SerializeField] public AudioClip TitleScreen { get; private set; }
        [field: SerializeField] public AudioClip Kingdom { get; private set; }
        [field: SerializeField] public AudioClip Fight { get; private set; }
    }
}