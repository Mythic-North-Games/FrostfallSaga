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
        [SerializeField] public MusicAudioClipsConfig musicAudioClipsConfig;
        [SerializeField, Range(0f, 3f)] private float defaultFadeDuration = 1.5f;
        private UIAudioClipSelector _uIAudioClipSelector;
        private MusicAudioClipSelector _musicAudioClipSelector;
        private AudioSource _currentFXAudioSource;
        private AudioSource _currentMusicAudioSource;



        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
            _musicAudioClipSelector = new MusicAudioClipSelector(musicAudioClipsConfig);
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
                Debug.LogWarning("Tried to play a null FX sound clip.");
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


        /// <summary>
        /// Play a Music sound effect by using the FXSounds enum.
        /// </summary>
        /// <param name="sound">The sound to play</param>
        /// <param name="volume">The volume of the audio clip</param>
        /// <param name="loop">Whether the sound should loop or not</param>
        public void PlayMusicSound(
            MusicSounds soundName,
            float volume,
            bool loop = true
        )
        {
            Debug.Log($"PlayMusicSound called for {soundName}");

            AudioClip sound = _musicAudioClipSelector.SelectAudioClip(soundName);

            if (sound == null)
            {
                Debug.LogWarning($"No music clip found for: {soundName}");
                return;
            }

            // Fade out current music
            if (_currentMusicAudioSource != null)
            {
                StartCoroutine(FadeOutAndDestroyAudioSource(_currentMusicAudioSource, defaultFadeDuration));
            }

            // Create and configure a new AudioSource for music
            AudioSource newAudioSource = Instantiate(audioSourceObject);
            newAudioSource.clip = sound;
            newAudioSource.volume = volume;
            newAudioSource.loop = loop;
            newAudioSource.Play();

            _currentMusicAudioSource = newAudioSource;

            // If not looping, destroy after the clip ends
            if (!loop) Destroy(newAudioSource.gameObject, sound.length);
        }

#if UNITY_EDITOR
        public void InitializeAudioClipSelectorFromTests(UIAudioClipsConfig uIAudioClipsConfig)
        {
            _uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
        }

#endif
    }
}