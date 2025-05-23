using System;

namespace FrostfallSaga.Core.Fight
{
    public enum EAbilityState
    {
        Locked,
        Unlockable,
        Unlocked
    }

    public static class EAbilityStateMethods
    {
        public static string GetUIString(this EAbilityState state)
        {
            return state switch
            {
                EAbilityState.Locked => "Dormant...",
                EAbilityState.Unlockable => "AWAKEN",
                EAbilityState.Unlocked => "AWOKEN",
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}