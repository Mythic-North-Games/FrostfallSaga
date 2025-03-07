using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        ///     Instance a singleton of the AudioManager in the scene
        /// </summary>
        public static AudioManager instance;

        [SerializeField] private UIAudioClipsConfig uiAudioClipsConfig;
        [SerializeField] private AudioSource audioSourceObject;
        private UIAudioClipSelector uIAudioClipSelector;

        private void Awake()
        {
            if (instance == null) instance = this;
            uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
        }

        /// <summary>
        ///     Create an audioSource gameObject in the scene and play the audioClip, then delete the gameObject
        ///     <paramref name="audioClip" /> The audio clip to play
        ///     <paramref name="spawnTransform" /> The transform to spawn the audioSource gameObject
        ///     <paramref name="audioVolume" /> The volume of the audio clip
        /// </summary>
        public void PlaySoundEffectClip(AudioClip audioClip, Transform spawnTransform, float audioVolume)
        {
            var clipLength = audioClip.length;
            AudioSource audioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = audioVolume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
        }

        /// <summary>
        ///     Play a UI sound effect by using the UISounds enum
        ///     <paramref name="sound" /> The sound to play
        /// </summary>
        public void PlayUISound(UISounds sound)
        {
            AudioClip audioClip = uIAudioClipSelector.SelectAudioClip(sound);
            if (audioClip != null)
                PlaySoundEffectClip(audioClip, transform, 1f);
            else
                Debug.LogError("Audio clip " + sound + " not found");
        }

#if UNITY_EDITOR


        public void InitializeAudioClipSelectorFromTests(UIAudioClipsConfig uIAudioClipsConfig)
        {
            uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
        }


#endif
    }
}