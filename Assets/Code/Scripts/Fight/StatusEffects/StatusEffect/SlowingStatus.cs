using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.StatusEffects
{
    [CreateAssetMenu(fileName = "New slowing Status", menuName = "Status Effect/Slowing")]
    public class SlowingStatus : StatusEffect
    {
        [SerializeField] private int speedReduction = 10;


        public override void ApplyStatusEffect(Fighter fighter)
        {
            fighter.ReduceStats(this, speedReduction, this.AnimationStateName);
            Debug.Log($"{fighter.name}'s speed is reduced by {speedReduction}.");
        }

        public override void RemoveStatusEffect(Fighter fighter)
        {
            fighter.IncreaseStats(this, speedReduction, this.AnimationStateName);
            Debug.Log($"{fighter.name}'s speed is back to normal.");
        }

        public int SpeedReduction
        {
            get { return speedReduction; }
            set { speedReduction = value; }
        }
    }

}
