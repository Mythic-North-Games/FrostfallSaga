using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils.Scenes;
using FrostfallSaga.Audio;

using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.TitleScreen
{
    public class TitleScreenMusicStarter : MonoBehaviour
    {
        private void Start()   
        {
            AudioManager.Instance.PlayMusicSound(MusicSounds.TitleScreen, 1.0f);
        }
    }
}