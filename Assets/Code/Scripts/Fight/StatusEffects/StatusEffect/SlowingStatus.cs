using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.StatusEffects
{

    public class SlowingStatus : StatusEffect
    {
        private int speedReduction;

        public SlowingStatus()
        {
            StatusType = StatusType.Slowed;
            Name = "Slowing";
            Description = "Reduces movement speed.";
            Duration = 3;
            animationStateName = "Slow";
            IsRecurring = false;
            this.speedReduction = 5;
        }

        public override void ApplyEffect(Fighter fighter)
        {
            fighter.ReduceStats(StatusType, speedReduction, this.animationStateName);
            Debug.Log($"{fighter.FighterName}'s speed is reduced by {speedReduction}.");
        }

        public override void RemoveEffect(Fighter fighter)
        {
            fighter.IncreaseStats(StatusType, speedReduction, this.animationStateName);
            Debug.Log($"{fighter.FighterName}'s speed is back to normal.");
        }
    }

}
