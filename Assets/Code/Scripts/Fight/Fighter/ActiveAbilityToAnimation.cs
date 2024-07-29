using System;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.Fight.Fighters
{
    [Serializable]
    public class ActiveAbilityToAnimation
    {
        public ActiveAbilitySO activeAbility;
        public AAbilityAnimationSO animation;
    }
}