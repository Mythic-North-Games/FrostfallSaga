using UnityEngine;

namespace FrostfallSaga.Audio
{
    [CreateAssetMenu(fileName = "MusicAudioClipsConfig", menuName = "ScriptableObjects/Audio/MusicAudioClipsConfig")]
    public class MusicAudioClipsConfig : ScriptableObject
    {
        public AudioClip titleScreenMusic;
        public AudioClip kingdomMusic;
        public AudioClip fightMusic;
    }
}