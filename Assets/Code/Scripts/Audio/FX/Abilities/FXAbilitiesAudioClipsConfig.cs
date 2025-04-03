using UnityEngine;

namespace FrostfallSaga.Audio
{

    [CreateAssetMenu(fileName = "FXAbilitiesAudioClipsConfig", menuName = "ScriptableObjects/Audio/Abilties/FXAbilitiesAudioClipsConfig")]
    public class FXAbilitiesAudioClipsConfig : ScriptableObject
    {
        public AudioClip fireBallAbilitySound;
        public AudioClip healAbilitySound;
        
    } 
}