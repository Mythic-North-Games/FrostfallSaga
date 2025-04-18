﻿using System;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    ///     Effect that applies heal to the target fighter.
    /// </summary>
    [Serializable]
    public class HealEffect : AEffect
    {
        [field: SerializeField] [field: Range(0, 9999)]
        public int HealAmount;

        public override void ApplyEffect(
            Fighter receiver,
            bool isMasterstroke,
            Fighter initiator = null,
            bool adjustGodFavorsPoints = true
        )
        {
            int finalHealAmount = HealAmount;

            // Increase heal amount if masterstroke
            if (isMasterstroke)
            {
                finalHealAmount = ApplyMasterstroke(HealAmount);
                Debug.Log($"Masterstroke succeeded, heal amount increased to {finalHealAmount}.");
            }

            // Apply the heal
            receiver.Heal(finalHealAmount, isMasterstroke);
            Debug.Log($"Healed {receiver.name} for {finalHealAmount} health.");

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints && initiator != null)
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.HEAL);
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Heal effects cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return 0;
        }

        public override int GetPotentialEffectHeal(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return HealAmount * (canMasterstroke ? 2 : 1);
        }

        public override string GetUIEffectDescription()
        {
            return $"Heals <b>{HealAmount}</b> health to target.";
        }
    }
}