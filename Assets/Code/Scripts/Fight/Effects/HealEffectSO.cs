﻿using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "HealEffect", menuName = "ScriptableObjects/Fight/Effects/HealEffect", order = 0)]
    public class HealEffectSO : AEffectSO
    {
        [field: SerializeField, Range(0, 9999)] public int HealAmount { get; private set; }

        public override void ApplyEffect(Fighter fighter)
        {
            fighter.Heal(HealAmount);
        }
    }
}
