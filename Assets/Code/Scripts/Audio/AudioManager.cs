using System.Collections;
using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class AudioManager : MonoBehaviour
    {
        // <summary>
        ///     Instance a singleton of the AudioManager in the scene
        /// </summary>
        public static AudioManager Instance;

        [SerializeField] private UIAudioClipsConfig uiAudioClipsConfig;
        [SerializeField] private AudioSource audioSourceObject;
        private AudioSource _currentFXAudioSource;
        private UIAudioClipSelector _uIAudioClipSelector;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
        }

        /// <summary>
        ///     Play a UI sound effect by using the UISounds enum
        /// </summary>
        /// <param name="sound">The sound to play</param>
        public void PlayUISound(UISounds sound)
        {
            AudioClip audioClip = _uIAudioClipSelector.SelectAudioClip(sound);
            if (audioClip != null) PlaySoundEffectClip(audioClip, transform, 1f);
            else Debug.LogError("Audio clip " + sound + " not found");
        }

        /// <summary>
        ///     Play a UI sound effect from a specific AudioClip
        /// </summary>
        /// <param name="sound">The sound to play</param>
        public void PlayUISound(AudioClip sound)
        {
            PlaySoundEffectClip(sound, transform, 1f);
        }

        /// <summary>
        /// Play a FX sound effect by using the FXSounds enum.
        /// </summary>
        /// <param name="sound">The sound to play</param>
        /// <param name="spawnTransform">The transform to spawn the audio source at</param>
        /// <param name="volume">The volume of the audio clip</param>
        /// <param name="fadeOutDuration">The duration of the fade out effect</param>
        /// <param name="loop">Whether the sound should loop or not</param>
        public void PlayFXSound(
            AudioClip sound,
            Transform spawnTransform,
            float volume,
            float fadeOutDuration,
            bool loop = false
        )
        {
            if (sound == null)
            {
                Debug.LogWarning("Tried to play a null sound clip.");
                return;
            }

            // Fade out the current sound if it is playing
            if (_currentFXAudioSource != null && _currentFXAudioSource.isPlaying)
            {
                StartCoroutine(FadeOutAndDestroyAudioSource(_currentFXAudioSource, fadeOutDuration));
            }

            // Create and configure a new AudioSource
            AudioSource newAudioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
            newAudioSource.clip = sound;
            newAudioSource.volume = volume;
            newAudioSource.loop = loop;
            _currentFXAudioSource = newAudioSource;

            // Play the new sound
            newAudioSource.Play();

            // Destroy the AudioSource after the sound has finished playing if not looping
            if (!newAudioSource.loop) Destroy(newAudioSource.gameObject, sound.length);
        }

        /// <summary>
        ///     Create an audioSource gameObject in the scene and play the audioClip, then delete the gameObject
        /// </summary>
        /// <param name="audioClip">The audio clip to play</param>
        /// <param name="spawnTransform">The transform to spawn the audio source at</param>
        /// <param name="audioVolume">The volume of the audio clip</param>
        public void PlaySoundEffectClip(AudioClip audioClip, Transform spawnTransform, float audioVolume)
        {
            float clipLength = audioClip.length;
            AudioSource audioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = audioVolume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
        }

        private static IEnumerator FadeOutAndDestroyAudioSource(AudioSource audioSource, float fadeDuration)
        {
            float startVolume = audioSource.volume;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                yield return null;
            }

            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }


        // A UTILISER POUR LA MUSIQUE
        //public void PlayMusicSound(FXSounds sound, Transform spawnTransform, float audioVolume, float duration)
        //{
        //AudioClip audioClip = fXAudioClipSelector.SelectAudioClip(sound);
        //if (audioClip != null)
        //{
        //float clipLength = audioClip.length;
        //float destroyTime = Mathf.Max(clipLength, duration); // On garde l'objet au moins le temps défini

        // Créer une source audio
        //AudioSource audioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
        //audioSource.clip = audioClip;
        //audioSource.volume = audioVolume;

        // Activez la lecture en boucle si la durée souhaitée est plus longue que la durée du clip
        //if (duration > audioClip.length)
        //{
        //    audioSource.loop = true;
        //}

        // Jouez le son
        //audioSource.Play();

        // Démarrer une coroutine pour arrêter le son après la durée spécifiée
        //StartCoroutine(StopSoundAfterDuration(audioSource, duration));

        //Destroy(audioSource.gameObject, destroyTime);
        //}
        //else
        //{
        //Debug.LogError("Audio clip " + sound + " not found");
        //}
        //}

#if UNITY_EDITOR
        public void InitializeAudioClipSelectorFromTests(UIAudioClipsConfig uIAudioClipsConfig)
        {
            _uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
        }
#endif
    }
}