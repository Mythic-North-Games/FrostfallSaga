using System;
using FrostfallSaga.Fight.Abilities;

namespace FrostfallSaga.Fight.Fighters
{
    [Serializable]
    public class ActiveAbilityToAnimation
    {
        public ActiveAbilitySO activeAbility;
        public string animationState;
    }
}