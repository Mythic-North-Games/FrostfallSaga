using UnityEngine;

namespace FrostfallSaga.Audio
{


    [CreateAssetMenu(fileName = "UIAudioClipsConfig", menuName = "ScriptableObjects/Audio/UIAudioClipsConfig")]
    public class UIAudioClipsConfig : ScriptableObject
    {
        public AudioClip fightBeginSound;
        public AudioClip buttonClickSound;
        public AudioClip buttonHoverSound;
        public AudioClip fightWonSound;
        public AudioClip fightLostSound;
    }      

}