using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Instance a singleton of the AudioManager in the scene
        /// </summary>
        public static AudioManager instance;

        [SerializeField] private AudioSource audioSourceObject;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        /// <summary>
        /// Create an audioSource gameObject in the scene and play the audioClip, then delete the gameObject
        /// <paramref name="audioClip"/> The audio clip to play
        /// <paramref name="spawnTransform"/> The transform to spawn the audioSource gameObject
        /// <paramref name="audioVolume"/> The volume of the audio clip
        /// </summary>
        public void PlaySoundEffectClip(AudioClip audioClip, Transform spawnTransform, float audioVolume)
        {
            float clipLength = audioClip.length;
            AudioSource audioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = audioVolume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
        }
    }
}
