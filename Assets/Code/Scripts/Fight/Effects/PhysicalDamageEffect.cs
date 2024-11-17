using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies physical damage to the target fighter.
    /// </summary>
    [Serializable]
    public class PhysicalDamageEffect : AEffect
    {
        [SerializeField, Range(0, 9999)] public int PhysicalDamageAmount;

        public override void ApplyEffect(
            Fighter receiver,
            Fighter initiator = null,
            bool canMasterstroke = true,
            bool canDodge = true,
            bool adjustGodFavorsPoints = true
        )
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(receiver))
            {
                Debug.Log($"{receiver.name} dodged heal effect.");
                return;
            }

            int finalDamageAmount = PhysicalDamageAmount;
            bool masterstrokeSucceeded = false;

            // Calculate masterstroke
            if (canMasterstroke && initiator != null)
            {
                finalDamageAmount = TryMasterstroke(initiator, PhysicalDamageAmount);
                masterstrokeSucceeded = finalDamageAmount != PhysicalDamageAmount;
                if (masterstrokeSucceeded)
                {
                    Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
                }
            }

            // Apply physical damage
            receiver.PhysicalWithstand(finalDamageAmount);
            receiver.onEffectReceived?.Invoke(receiver, initiator, this, masterstrokeSucceeded);

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints && initiator != null)
            {
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.DAMAGE);
            }
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Physical damage effects cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return PhysicalDamageAmount * (canMasterstroke ? 2 : 1);
        }
    }
}