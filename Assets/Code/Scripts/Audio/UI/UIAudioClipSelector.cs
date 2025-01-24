using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class UIAudioClipSelector : MonoBehaviour
    {
        private UIAudioClipsConfig uIAudioClipsConfig;

        public UIAudioClipSelector(UIAudioClipsConfig config){
            uIAudioClipsConfig = config;
        }

        public AudioClip SelectAudioClip(UISounds soundName){
            switch (soundName)
            {
                case UISounds.FightBegin:
                    return uIAudioClipsConfig.fightBeginSound;
                case UISounds.ButtonClick:
                    return uIAudioClipsConfig.buttonClickSound;
                case UISounds.ButtonHover:
                    return uIAudioClipsConfig.buttonHoverSound;
                case UISounds.FightWon:
                    return uIAudioClipsConfig.fightWonSound;
                case UISounds.FightLost:
                    return uIAudioClipsConfig.fightLostSound;
                default:
                    return null;
            }
        }
    }
}
