using System.Collections;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class AudioManager : MonoBehaviourPersistingSingleton<AudioManager>
    {
        // Default audio settings
        private static readonly float DEFAULT_UI_VOLUME = 1f;
        private static readonly float DEFAULT_MUSIC_VOLUME = 0.25f;
        private static readonly float DEFAULT_FX_VOLUME = 1f;
        private static readonly float DEFAULT_MUSIC_FADE_IN_DURATION = 5f;
        private static readonly float DEFAULT_MUSIC_FADE_OUT_DURATION = 2.5f;

        // Paths to the audio resources
        private const string UI_AUDIO_CLIPS_RESOURCE_PATH = "ScriptableObjects/Audio/UIAudioClips";
        private const string MUSIC_AUDIO_CLIPS_RESOURCE_PATH = "ScriptableObjects/Audio/MusicAudioClips";
        private const string FX_AUDIO_SOURCE_PREFAB_PATH = "Prefabs/Audio/FXAudioSource";

        // Audio clips
        public UIAudioClips UIAudioClips { get; private set; }
        public MusicAudioClips MusicAudioClips { get; private set; }

        // Audio sources
        private AudioSource _uiAudioSource;
        private AudioSource _musicAudioSource;
        private AudioSource _fxAudioSourcePrefab;

        private void Start()
        {
            // Apply default volume settings
            _uiAudioSource.volume = DEFAULT_UI_VOLUME;
            _musicAudioSource.volume = DEFAULT_MUSIC_VOLUME;
            _fxAudioSourcePrefab.volume = DEFAULT_FX_VOLUME;
        }

        /// <summary>
        /// Play a UI sound effect (2d sound).
        /// </summary>
        /// <param name="uiAudioClip">The sound to play</param>
        public void PlayUISound(AudioClip uiAudioClip)
        {
            _uiAudioSource.PlayOneShot(uiAudioClip);
        }

        /// <summary>
        /// Play a music (2d sound) looping by default. Stops any current music.
        /// If the same music is already playing, it will not be played again.
        /// </summary>
        /// <param name="musicAudioClip">The music to play.</param>
        /// <param name="enableLooping">Whether the music should loop or not.</param>
        /// <param name="fadeDuration">The duration of the fade in and out effect before playing the new music.</param>
        public void PlayMusicSound(AudioClip musicAudioClip, bool enableLooping = true, float fadeDuration = -99f)
        {
            // Don't play the same music again
            if (_musicAudioSource.clip == musicAudioClip) return;

            if (_musicAudioSource.isPlaying)
            {
                // Fade out the current music
                StartCoroutine(FadeOutMusicAndPlayNewOne(
                    musicAudioClip,
                    fadeDuration == -99f ? DEFAULT_MUSIC_FADE_OUT_DURATION : fadeDuration
                ));
            }
            else
            {
                // Find in the new music clip
                StartCoroutine(FadeInMusic(
                    musicAudioClip,
                    enableLooping,
                    fadeDuration == -99f ? DEFAULT_MUSIC_FADE_IN_DURATION : fadeDuration
                ));
            }
        }

        /// <summary>
        /// Stop the current music sound.
        /// If fadeOutDuration is 0, the music will stop immediately.
        /// </summary>
        /// <param name="fadeOutDuration">The custom duration of the fade out effect.</param>
        public void StopCurrentMusic(float fadeOutDuration = -99f)
        {
            if (fadeOutDuration == 0f)
            {
                _musicAudioSource.Stop();
                return;
            }

            // Fade out the current music
            StartCoroutine(FadeOutMusic(
                fadeOutDuration == -99f ? DEFAULT_MUSIC_FADE_OUT_DURATION : fadeOutDuration
            ));
        }

        /// <summary>
        /// Play a FX sound effect by using the FXSounds enum.
        /// </summary>
        /// <param name="sound">The sound to play</param>
        /// <param name="spawnTransform">The transform to spawn the audio source at</param>
        /// <param name="enableLooping">Whether the sound should loop or not</param>
        public void PlayFXSound(
            AudioClip sound,
            Transform spawnTransform,
            bool enableLooping = false
        )
        {
            // Create and configure a new AudioSource
            AudioSource newAudioSource = Instantiate(_fxAudioSourcePrefab, spawnTransform.position, Quaternion.identity);
            newAudioSource.clip = sound;
            newAudioSource.loop = enableLooping;
            newAudioSource.volume = DEFAULT_FX_VOLUME;

            // Play the new sound
            newAudioSource.Play();

            // Destroy the AudioSource after the sound has finished playing if not looping
            if (!newAudioSource.loop) Destroy(newAudioSource.gameObject, sound.length);
        }

        private IEnumerator FadeInMusic(AudioClip musicAudioClip, bool enableLooping, float fadeInDuration)
        {
            float startVolume = 0f;

            _musicAudioSource.clip = musicAudioClip;
            _musicAudioSource.loop = enableLooping;
            _musicAudioSource.volume = startVolume; // Start with volume 0
            _musicAudioSource.Play();

            for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
            {
                _musicAudioSource.volume = Mathf.Lerp(startVolume, DEFAULT_MUSIC_VOLUME, t / fadeInDuration);
                yield return null;
            }

            _musicAudioSource.volume = DEFAULT_MUSIC_VOLUME;
        }

        private IEnumerator FadeOutMusic(float fadeOutDuration)
        {
            float startVolume = _musicAudioSource.volume;

            for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
            {
                _musicAudioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
                yield return null;
            }

            _musicAudioSource.Stop();
            _musicAudioSource.volume = startVolume; // Reset volume to original
        }

        private IEnumerator FadeOutMusicAndPlayNewOne(AudioClip newMusicClip, float fadeOutDuration)
        {
            StartCoroutine(FadeOutMusic(fadeOutDuration));
            yield return new WaitForSeconds(fadeOutDuration);
            PlayMusicSound(newMusicClip);
        }

        #region Initialization
        protected override void Init()
        {
            // Load the audio clips
            UIAudioClips = Resources.Load<UIAudioClips>(UI_AUDIO_CLIPS_RESOURCE_PATH);
            MusicAudioClips = Resources.Load<MusicAudioClips>(MUSIC_AUDIO_CLIPS_RESOURCE_PATH);
            if (UIAudioClips == null || MusicAudioClips == null)
            {
                Debug.LogError("Audio clips not found at specified paths.");
                return;
            }

            // Create the audio sources
            if (_uiAudioSource == null) _uiAudioSource = gameObject.AddComponent<AudioSource>();
            if (_musicAudioSource == null) _musicAudioSource = gameObject.AddComponent<AudioSource>();

            // Set the audio source properties for 2d sound
            _uiAudioSource.spatialBlend = 0f;
            _uiAudioSource.reverbZoneMix = 0f;
            _musicAudioSource.spatialBlend = 0f;
            _musicAudioSource.reverbZoneMix = 0f;

            // Load the FX audio source prefab
            _fxAudioSourcePrefab = Resources.Load<AudioSource>(FX_AUDIO_SOURCE_PREFAB_PATH);
            if (_fxAudioSourcePrefab == null)
            {
                Debug.LogError("FX Audio Source prefab not found at specified path.");
                return;
            }
        }
        #endregion
    }
}