using System;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    ///     Effect that applies physical damage to the target fighter.
    /// </summary>
    [Serializable]
    public class PhysicalDamageEffect : AEffect
    {
        [SerializeField] [Range(0, 9999)] public int PhysicalDamageAmount;

        public PhysicalDamageEffect()
        {
        }

        public PhysicalDamageEffect(int physicalDamageAmount)
        {
            PhysicalDamageAmount = physicalDamageAmount;
        }

        public override void ApplyEffect(
            Fighter receiver,
            bool isMasterstroke,
            Fighter initiator = null,
            bool adjustGodFavorsPoints = true
        )
        {
            int finalDamageAmount = PhysicalDamageAmount;

            // Increase damage amount if masterstroke
            if (isMasterstroke && initiator != null)
            {
                finalDamageAmount = ApplyMasterstroke(PhysicalDamageAmount);
                Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
            }

            // Apply physical damage
            receiver.PhysicalWithstand(finalDamageAmount, isMasterstroke);

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints && initiator != null)
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.DAMAGE);
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Physical damage effects cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return PhysicalDamageAmount * (canMasterstroke ? 2 : 1);
        }
        public override int GetPotentialEffectHeal(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return 0;
        }

        public override string GetUIEffectDescription()
        {
            return $"Deals <color=red><b>{PhysicalDamageAmount}</b></color> physical damage to target.";
        }
    }
}