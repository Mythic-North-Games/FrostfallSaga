using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class AutoFXSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip soundFX;
        [SerializeField] private float volume = 1f;
        [SerializeField] private float fadeOutDuration = 1f;
        [SerializeField] private bool loop = false;

        private void Start()
        {
            AudioManager.Instance.PlayFXSound(soundFX, transform, volume, fadeOutDuration, loop);
        }
    }
}