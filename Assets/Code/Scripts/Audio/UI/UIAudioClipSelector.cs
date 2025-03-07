using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class UIAudioClipSelector
    {
        private readonly UIAudioClipsConfig _uIAudioClipsConfig;

        public UIAudioClipSelector(UIAudioClipsConfig uIAudioClipsConfig)
        {
            _uIAudioClipsConfig = uIAudioClipsConfig;
        }

        public AudioClip SelectAudioClip(UISounds soundName)
        {
            if (_uIAudioClipsConfig == null)
            {
                Debug.LogError("UIAudioClipsConfig n'est pas assigné dans UIAudioClipSelector !");
                return null;
            }

            switch (soundName)
            {
                case UISounds.FightBegin:
                    return _uIAudioClipsConfig.fightBeginSound;
                case UISounds.ButtonClick:
                    return _uIAudioClipsConfig.buttonClickSound;
                case UISounds.ButtonHover:
                    return _uIAudioClipsConfig.buttonHoverSound;
                case UISounds.FightWon:
                    return _uIAudioClipsConfig.fightWonSound;
                case UISounds.FightLost:
                    return _uIAudioClipsConfig.fightLostSound;
                default:
                    return null;
            }
        }
    }
}