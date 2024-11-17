using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [Serializable]
    public class HealEffect : AEffect
    {
        [field: SerializeField, Range(0, 9999)] public int HealAmount;

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

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints && initiator != null)
            {
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.HEAL);
            }
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Heal effects cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return 0;
        }
    }
}