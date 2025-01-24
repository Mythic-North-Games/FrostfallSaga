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

        [SerializeField] private UIAudioClipsConfig uiAudioClipsConfig;
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

        /// <summary>
        /// Play a UI sound effect by using the UISounds enum
        /// <paramref name="sound"/> The sound to play
        /// </summary>
        public void PlayUISound(UISounds sound)
        {
            AudioClip audioClip;
            switch (sound)
            {
                case UISounds.FightBegin:
                    audioClip = uiAudioClipsConfig.fightBeginSound;
                    break;
                case UISounds.ButtonClick:
                    audioClip = uiAudioClipsConfig.buttonClickSound;
                    break;
                case UISounds.ButtonHover:
                    audioClip = uiAudioClipsConfig.buttonHoverSound;
                    break;
                case UISounds.FightWon:
                    audioClip = uiAudioClipsConfig.fightWonSound;
                    break;
                case UISounds.FightLost:
                    audioClip = uiAudioClipsConfig.fightLostSound;
                    break;
                default:
                    audioClip = null;
                    break;
            }

            if(audioClip != null){
                PlaySoundEffectClip(audioClip, transform, 1f);
            } else {
                Debug.LogError("Audio clip " + sound +" not found");
            }
        }
    }
}
