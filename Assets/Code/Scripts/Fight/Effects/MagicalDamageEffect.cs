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

        public MagicalDamageEffect() {}

        public MagicalDamageEffect(int magicalDamageAmount, EMagicalElement magicalElement)
        {
            MagicalDamageAmount = magicalDamageAmount;
            MagicalElement = magicalElement;
        }

        public override void ApplyEffect(
            Fighter receiver,
            bool isMasterstroke,
            Fighter initiator = null,
            bool adjustGodFavorsPoints = true
        )
        {
            int finalDamageAmount = MagicalDamageAmount;

            // Increase heal amount if masterstroke
            if (isMasterstroke && initiator != null)
            {
                finalDamageAmount = ApplyMasterstroke(MagicalDamageAmount);
                Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
            }

            // Apply magical damage
            receiver.MagicalWithstand(finalDamageAmount, MagicalElement, isMasterstroke);
            Debug.Log($"Dealt {finalDamageAmount} magical damage of {MagicalElement} to {receiver.name}.");

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints  && initiator != null)
            {
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.DAMAGE);
            }
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Magical damage effects cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return MagicalDamageAmount * (canMasterstroke ? 2 : 1);
        }
    }
}