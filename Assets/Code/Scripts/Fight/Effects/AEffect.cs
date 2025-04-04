using System;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    ///     Base class for all effects that abilities will have.
    /// </summary>
    [Serializable]
    public abstract class AEffect
    {
        /// <summary>
        ///     Apply the effect to the target fighter using the initiator stats for possible masterstroke and dodge chance.
        /// </summary>
        /// <param name="receiver">The fighter that will receive the effect.</param>
        /// <param name="initiator">The fighter that initiates the effect if there is one.</param>
        /// <param name="isMasterstroke">True if the effect can be masterstroked, false otherwise.</param>
        /// <param name="adjustGodFavorsPoints">
        ///     True if the god favors points should be adjusted for the initiator, false
        ///     otherwise.
        /// </param>
        public abstract void ApplyEffect(
            Fighter receiver,
            bool isMasterstroke,
            Fighter initiator = null,
            bool adjustGodFavorsPoints = true
        );

        /// <summary>
        ///     Restore the effect previously applied to the given fighter.
        /// </summary>
        /// <param name="receiver">The fighter that received the effect.</param>
        public abstract void RestoreEffect(Fighter receiver);

        /// <summary>
        ///     Make the given fighter try to masterstroke a given value.
        /// </summary>
        /// <param name="attacker">The fighter that tries to masterstroke.</param>
        /// <param name="baseValue">The base value before the potential masterstroke.</param>
        /// <returns>Either the base value if masterstroke failed or the augmented value.</returns>
        protected static int ApplyMasterstroke(int baseValue)
        {
            // Random multiplier between 1.5 and 2.0 for critical hit damage
            float criticalMultiplier = 1.5f + Random.value * 0.5f;
            int finalValue = Mathf.RoundToInt(baseValue * criticalMultiplier);
            Debug.Log($"Masterstroke! Damage multiplied by {criticalMultiplier:F2}, final damage: {finalValue}");
            return finalValue;
        }

        /// <summary>
        ///     Compute the maximum potential damage that the effect can do.
        /// </summary>
        /// <param name="initiator">The fighter that initiates the effect.</param>
        /// <param name="receiver">The fighter that will receive the effect.</param>
        /// <param name="canMasterstroke">True if the effect can be masterstroked, false otherwise.</param>
        /// <returns>Returns the maximum potential damage that the effect can do.</returns>
        public abstract int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke);

        /// Compute the maximum potential heal that the effect can do.
        /// </summary>
        /// <param name="initiator">The fighter that initiates the effect.</param>
        /// <param name="receiver">The fighter that will receive the effect.</param>
        /// <param name="canMasterstroke">True if the effect can be masterstroked, false otherwise.</param>
        /// <returns>Returns the maximum potential heal that the effect can do.</returns>
        public abstract int GetPotentialEffectHeal(Fighter initiator, Fighter receiver, bool canMasterstroke);

        /// <summary>
        /// Get the description of the effect for the UI.
        /// </summary>
        /// <returns>Returns the description of the effect for the UI.</returns>
        public abstract string GetUIEffectDescription();
    }
}