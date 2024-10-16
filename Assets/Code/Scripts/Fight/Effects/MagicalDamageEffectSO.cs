﻿using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies magical damage to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "MagicalDamageEffect", menuName = "ScriptableObjects/Fight/Effects/MagicalDamageEffect", order = 0)]
    public class MagicalDamageEffectSO : AEffectSO
    {
        [field: SerializeField, Range(0, 9999)] public int MagicalDamageAmount { get; private set; }
        [field: SerializeField] public EMagicalElement MagicalElement { get; private set; }

        public override void ApplyEffect(Fighter initiator, Fighter receiver, bool canMasterstroke = true, bool canDodge = true)
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(receiver))
            {
                Debug.Log($"{receiver.name} dodged heal effect.");
                return;
            }

            int finalDamageAmount = MagicalDamageAmount;
            bool masterstrokeSucceeded = false;

            // Calculate masterstroke
            if (canMasterstroke)
            {
                finalDamageAmount = TryMasterstroke(initiator, MagicalDamageAmount);
                masterstrokeSucceeded = finalDamageAmount != MagicalDamageAmount;
                if (masterstrokeSucceeded)
                {
                    Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
                }
            }

            receiver.MagicalWithstand(finalDamageAmount, MagicalElement);
            receiver.onEffectReceived?.Invoke(receiver, initiator, this, masterstrokeSucceeded);
            Debug.Log($"Dealt {finalDamageAmount} magical damage of {MagicalElement} to {receiver.name}.");
        }
    }
}