using System;
using FrostfallSaga.Fight.Abilities;
using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [Serializable]
    public class ActiveAbilitiesToAnimation
    {
        public ActiveAbilityScriptableObject activeAbility;
        public Animation animation;
    }
}