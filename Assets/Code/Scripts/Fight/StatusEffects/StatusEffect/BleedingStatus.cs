using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.StatusEffects
{

    public class BleedingStatus : StatusEffect
    {
        private int bleedingReduction;

        public BleedingStatus()
        {
            StatusType = StatusType.Bleeding;
            Name = "Bleeding";
            Description = "Causes damage over time.";
            Duration = 3;
            animationStateName = "Bleed";
            IsRecurring = true;
            this.bleedingReduction = 5;
        }

        public override void ApplyEffect(Fighter fighter)
        {
            fighter.inflictDamage(bleedingReduction, this.animationStateName);
            Debug.Log($"{fighter.FighterName} is bleeding and loses ${bleedingReduction} HP! ==> Health : ${fighter.GetHealth()}");
        }

        public override void RemoveEffect(Fighter fighter)
        {
            Debug.Log($"{fighter.FighterName} stopped bleeding.");
        }
    }

}
