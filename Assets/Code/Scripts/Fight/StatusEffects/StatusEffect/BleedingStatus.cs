using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.StatusEffects
{
    [CreateAssetMenu(fileName = "New Bleeding Status", menuName = "Status Effect/Bleeding")]
    public class BleedingStatus : StatusEffect
    {
        [SerializeField] private int bleedingReduction = 10;

        public override void ApplyStatusEffect(Fighter fighter)
        {
            fighter.inflictDamage(bleedingReduction, this.AnimationStateName);
            Debug.Log($"{fighter.name} is bleeding and loses ${bleedingReduction} HP! ==> Health : ${fighter.GetHealth()}");
        }

        public override void RemoveStatusEffect(Fighter fighter)
        {
            Debug.Log($"{fighter.name} stopped bleeding.");
        }


        public int BleedingReduction
        {
            get { return bleedingReduction; }
            set { bleedingReduction = value; }
        }
    }

}
