using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.StatusEffects
{

public class BleedingEffect : StatusEffect {
    private int bleedingReduction;

    public BleedingEffect() {
        StatusType = StatusType.Bleeding;
        Name = "Bleeding";
        Description = "Causes damage over time.";
        Duration = 3;
        animationStateName = "Bleed";
        IsRecurring = true;
        this.bleedingReduction =5;
    }

    public override void ApplyEffect(Fighter fighter) {
        fighter.inflictDamage(bleedingReduction, this.animationStateName); 
        Debug.Log($"{fighter.FighterName} is bleeding and loses ${bleedingReduction} HP! ==> Health : ${fighter.GetHealth()}");
    }

    public override void RemoveEffect(Fighter fighter) {
        Debug.Log($"{fighter.FighterName} stopped bleeding.");
    }
}

}
