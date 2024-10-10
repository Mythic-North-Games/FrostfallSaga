using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.StatusEffects
{
    [CreateAssetMenu(fileName = "New weakness Status", menuName = "Status Effect/Weakness")]
    public class WeaknessStatus : StatusEffect
    {
        [SerializeField] private int strengthReduction = 3;

        public override void ApplyStatusEffect(Fighter fighter)
        {
            fighter.ReduceStats(this, strengthReduction, this.AnimationStateName);
            Debug.Log($"{fighter.name}'s strength is reduced by {strengthReduction} == > Strength : ${fighter.GetStrength()}.");
        }

        public override void RemoveStatusEffect(Fighter fighter)
        {
            fighter.IncreaseStats(this, strengthReduction, this.AnimationStateName);
            Debug.Log($"{fighter.name}'s strength is back to normal !");
        }

        public int StrengthReduction
        {
            get { return strengthReduction; }
            set { strengthReduction = value; }
        }
    }

}
