using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class AutoFXSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip soundFX;
        [SerializeField] private bool enableLooping = false;

        private void Start()
        {
            AudioManager.Instance.PlayFXSound(soundFX, transform, enableLooping);
        }
    }
}