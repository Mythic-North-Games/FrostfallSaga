using UnityEngine;

namespace FrostfallSaga.Audio
{
    public class MusicAudioClipSelector
    {
        private readonly MusicAudioClipsConfig _musicAudioClipsConfig;

        public MusicAudioClipSelector(MusicAudioClipsConfig musicAudioClipsConfig)
        {
            _musicAudioClipsConfig = musicAudioClipsConfig;
        }

        public AudioClip SelectAudioClip(MusicSounds soundName)
        {
            if (_musicAudioClipsConfig == null)
            {
                Debug.LogError("MusicAudioClipsConfig n'est pas assigné dans MusicAudioClipSelector !");
                return null;
            }

            switch (soundName)
            {
                case MusicSounds.TittleScreen:
                    return _musicAudioClipsConfig.tittleScreenMusic;
                case MusicSounds.Kingdom:
                    return _musicAudioClipsConfig.kingdomMusic;
                case MusicSounds.Fight:
                    return _musicAudioClipsConfig.fightMusic;
                default:
                    return null;
            }
        }
    }
}