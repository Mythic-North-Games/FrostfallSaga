using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.StatusEffects
{

public class WeaknessStatus : StatusEffect {
    private int strengthReduction;

    public void SlowingStatus() {
        StatusType = StatusType.Slowed;
        Name = "Weakness";
        Description = "Reduces attack power.";
        Duration = 3;
        animationStateName = "Weakness";
        IsRecurring = false;
        this.strengthReduction =5;

    }

    public override void ApplyEffect(Fighter fighter) {
        fighter.ReduceStats(StatusType, strengthReduction, this.animationStateName); 
        Debug.Log($"{fighter.FighterName}'s strength is reduced by {strengthReduction}.");
    }

    public override void RemoveEffect(Fighter fighter) {
        fighter.IncreaseStats(StatusType, strengthReduction, this.animationStateName); 
        Debug.Log($"{fighter.FighterName}'s strength is back to normal.");
    }
}

}
