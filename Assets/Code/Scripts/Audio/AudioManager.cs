using System.Collections;
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
        private UIAudioClipSelector _uIAudioClipSelector;
        private AudioSource _currentFXAudioSource;
        private Coroutine _fadeOutCoroutine;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            _uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);

            // Créer une source audio persistante
            _currentFXAudioSource = Instantiate(audioSourceObject, transform);
            _currentFXAudioSource.gameObject.name = "PersistentFXAudioSource";
            DontDestroyOnLoad(_currentFXAudioSource.gameObject);
        }

        /// <summary>
        ///     Create an audioSource gameObject in the scene and play the audioClip, then delete the gameObject
        ///     <paramref name="audioClip" /> The audio clip to play
        ///     <paramref name="spawnTransform" /> The transform to spawn the audioSource gameObject
        ///     <paramref name="audioVolume" /> The volume of the audio clip
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
        ///     Play a UI sound effect by using the UISounds enum
        ///     <paramref name="sound" /> The sound to play
        /// </summary>
        public void PlayUISound(UISounds sound)
        {
            AudioClip audioClip = _uIAudioClipSelector.SelectAudioClip(sound);
            if (audioClip != null)
                PlaySoundEffectClip(audioClip, transform, 1f);
            else
                Debug.LogError("Audio clip " + sound + " not found");
        }

        /// <summary>
        /// Play a FX sound effect by using the FXSounds enum
        /// <paramref name="sound"/> The sound to play
        /// </summary>
        /// 

        public void PlayFXSound(AudioClip sound, Transform spawnTransform, float volume, float durationFadeOut)
        {
            if (sound == null)
                return;

            // Arrêter le fade-out en cours si il y en a un
            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
                _fadeOutCoroutine = null;
            }

            // Démarrer le fade-out du son actuel
            if (_currentFXAudioSource != null && _currentFXAudioSource.isPlaying)
            {
                _fadeOutCoroutine = StartCoroutine(FadeOutAndDestroy(_currentFXAudioSource, durationFadeOut));
            }

            // Création d'une nouvelle AudioSource pour le nouveau son
            AudioSource newAudioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
            newAudioSource.clip = sound;
            newAudioSource.volume = volume;
            newAudioSource.Play();

            // Définir cette nouvelle source comme l'actuelle
            _currentFXAudioSource = newAudioSource;

            // Détruire l'AudioSource après la fin du son
            Destroy(newAudioSource.gameObject, sound.length);
        }

        // Coroutine pour effectuer un fade-out progressif et ensuite détruire l'AudioSource
        private IEnumerator FadeOutAndDestroy(AudioSource audioSource, float fadeDuration)
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

        private IEnumerator StopSoundAfterDuration(AudioSource audioSource, float duration)
        {
            yield return new WaitForSeconds(duration); // Attendre la durée spécifiée
            audioSource.Stop(); // Arrêter la lecture
            Destroy(audioSource.gameObject); // Détruire l'objet audio
        }

#if UNITY_EDITOR
        public void InitializeAudioClipSelectorFromTests(UIAudioClipsConfig uIAudioClipsConfig)
        {
            _uIAudioClipSelector = new UIAudioClipSelector(uiAudioClipsConfig);
        }
#endif
    }
}