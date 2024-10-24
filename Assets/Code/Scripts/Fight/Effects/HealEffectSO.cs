﻿using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "HealEffect", menuName = "ScriptableObjects/Fight/Effects/HealEffect", order = 0)]
    public class HealEffectSO : AEffectSO
    {
        [field: SerializeField, Range(0, 9999)] public int HealAmount { get; private set; }

        public override void ApplyEffect(Fighter initiator, Fighter receiver, bool canMasterstroke = true, bool canDodge = true)
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(receiver))
            {
                Debug.Log($"{receiver.name} dodged heal effect.");
                return;
            }

            int finalHealAmount = HealAmount;
            bool masterstrokeSucceeded = false;

            // Calculate masterstroke heal
            if (canMasterstroke)
            {
                finalHealAmount = TryMasterstroke(initiator, HealAmount);
                masterstrokeSucceeded = finalHealAmount != HealAmount;
                if (masterstrokeSucceeded)
                {
                    Debug.Log($"Masterstroke succeeded, heal amount increased to {finalHealAmount}.");
                }
            }

            // Apply the heal
            receiver.Heal(finalHealAmount);
            receiver.onEffectReceived?.Invoke(receiver, initiator, this, masterstrokeSucceeded);
            Debug.Log($"Healed {receiver.name} for {finalHealAmount} health.");
        }
    }
}