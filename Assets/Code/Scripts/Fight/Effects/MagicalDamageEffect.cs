using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies magical damage to the target fighter.
    /// </summary>
    [Serializable]
    public class MagicalDamageEffect : AEffect
    {
        [SerializeField, Range(0, 9999)] public int MagicalDamageAmount;
        [SerializeField] public EMagicalElement MagicalElement;

        public override void ApplyEffect(
            Fighter receiver,
            Fighter initiator = null,
            bool canMasterstroke = true,
            bool canDodge = true
        )
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
            if (canMasterstroke && initiator != null)
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

        public override void RestoreEffect(Fighter receiver)
        {
            // Magical damage effects cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return MagicalDamageAmount * (canMasterstroke ? 2 : 1);
        }
    }
}