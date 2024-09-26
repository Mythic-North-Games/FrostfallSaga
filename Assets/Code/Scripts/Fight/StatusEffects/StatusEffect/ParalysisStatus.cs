using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.StatusEffects
{

    public class ParalysisStatus : StatusEffect
    {
        public ParalysisStatus()
        {
            StatusType = StatusType.Paralysis;
            Name = "Paralysis";
            Description = "Prevents the fighter from performing actions.";
            Duration = 2;
            IsRecurring = false;
            animationStateName = "paralysis";

        }

        public override void ApplyEffect(Fighter fighter)
        {
            fighter.SetParalyzed(true);
            Debug.Log($"{fighter.FighterName} is paralyzed!");
        }

        public override void RemoveEffect(Fighter fighter)
        {
            fighter.SetParalyzed(false);
            Debug.Log($"{fighter.FighterName} is no longer paralyzed!");
        }
    }


}
