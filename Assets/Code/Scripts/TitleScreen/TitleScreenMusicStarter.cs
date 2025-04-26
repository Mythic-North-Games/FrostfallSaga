using FrostfallSaga.Audio;
using UnityEngine;

namespace FrostfallSaga.TitleScreen
{
    public class TitleScreenMusicStarter : MonoBehaviour
    {
        [SerializeField] private float _timeBeforeMusicStarts = 1f;

        private void Start()
        {
            StartCoroutine(WaitAndPlayTitleScreenMusic());
        }

        private System.Collections.IEnumerator WaitAndPlayTitleScreenMusic()
        {
            // Wait for the AudioManager to be initialized
            yield return new WaitForSeconds(_timeBeforeMusicStarts);

            // Play the title screen music
            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlayMusic(audioManager.MusicAudioClips.TitleScreen, true);
        }
    }
}