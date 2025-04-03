using UnityEngine;

namespace FrostfallSaga.Audio
{

    [CreateAssetMenu(fileName = "FXRunAudioClipsConfig", menuName = "ScriptableObjects/Audio/Run/FXRunAudioClipsConfig")]
    public class FXRunAudioClipsConfig : ScriptableObject
    {
        public AudioClip grassRunSound;
        public AudioClip snowRunSound;
        public AudioClip rockRunSound;
        public AudioClip waterRunSound;
        
    } 
}