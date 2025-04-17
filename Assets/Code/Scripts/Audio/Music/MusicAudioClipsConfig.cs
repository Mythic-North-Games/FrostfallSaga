using UnityEngine;

namespace FrostfallSaga.Audio
{
    [CreateAssetMenu(fileName = "MusicAudioClipsConfig", menuName = "ScriptableObjects/Audio/MusicAudioClipsConfig")]
    public class MusicAudioClipsConfig : ScriptableObject
    {
        public AudioClip tittleScreenMusic;
        public AudioClip kingdomMusic;
        public AudioClip fightMusic;
    }
}