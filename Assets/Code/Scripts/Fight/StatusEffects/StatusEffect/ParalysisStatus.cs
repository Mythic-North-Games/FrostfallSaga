using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using System;

namespace FrostfallSaga.Fight.StatusEffects
{
    [CreateAssetMenu(fileName = "New paralysis Status", menuName = "Status Effect/Paralysis")]
    public class ParalysisStatus : StatusEffect
    {
        public override void ApplyStatusEffect(Fighter fighter)
        {
            fighter.SetParalyzed(true);
            Debug.Log($"{fighter.name} is paralyzed!");
        }

        public override void RemoveStatusEffect(Fighter fighter)
        {
            fighter.SetParalyzed(false);
            Debug.Log($"{fighter.name} is no longer paralyzed!");
        }
    }


}
