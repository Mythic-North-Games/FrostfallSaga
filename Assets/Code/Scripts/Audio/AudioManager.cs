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
        /// </summary>
        public void PlaySoundEffectClip(AudioClip audioClip, Transform spawnTransform, float volume)
        {
            float clipLength = audioClip.length;
            AudioSource audioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
        }
    }
}
