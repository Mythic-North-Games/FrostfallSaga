using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Core;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Base class for all effects that abilities will have.
    /// </summary>
    [Serializable]
    public abstract class AEffect
    {
        [SerializeField] public string Name;
        [SerializeField] public string Description;
        [SerializeField] public bool Dodgable;
        [SerializeField] public bool Masterstrokable;

        /// <summary>
        /// Apply the effect to the target fighter using the initator stats for possible masterstroke and dodge chance.
        /// </summary>
        /// <param name="receiver">The fighter that will receive the effect.</param>
        /// <param name="initator">The fighter that initiates the effect if there is one.</param>
        /// <param name="canMasterstroke">True if the effect can be masterstroked, false otherwise.</param>
        /// <param name="canDodge">True if the effect can be dodged, false otherwise.</param>
        public abstract void ApplyEffect(
            Fighter receiver,
            Fighter initator = null,
            bool canMasterstroke = true,
            bool canDodge = true
        );

        /// <summary>
        /// Make the given fighter try to dodge something.
        /// </summary>
        /// <param name="defender">The fighter that tries to dodge.</param>
        /// <returns>True if the dodge is successfull, false otherwise</returns>
        public static bool TryDodge(Fighter defender)
        {
            return Randomizer.GetBooleanOnChance(defender.GetDodgeChance());
        }

        /// <summary>
        /// Make the given fighter try to masterstroke a given value.
        /// </summary>
        /// <param name="attacker">The fighter that tries to masterstroke.</param>
        /// <param name="baseValue">The base value before the potential masterstroke.</param>
        /// <returns>Either the base value if masterstroke failed or the augmented value.</returns>
        public static int TryMasterstroke(Fighter attacker, int baseValue)
        {
            if (Randomizer.GetBooleanOnChance(attacker.GetMasterstrokeChance()))
            {
                // Random multiplier between 1.5 and 2.0 for critical hit damage
                float criticalMultiplier = 1.5f + (UnityEngine.Random.value * 0.5f);
                int finalValue = Mathf.RoundToInt(baseValue * criticalMultiplier);
                Debug.Log($"Masterstroke! Damage multiplied by {criticalMultiplier:F2}, final damage: {finalValue}");
                return finalValue;
            }
            return baseValue;
        }

        /// <summary>
        /// Compute the maximum potential damage that the effect can do.
        /// </summary>
        /// <param name="initiator">The fighter that initiates the effect.</param>
        /// <param name="receiver">The fighter that will receive the effect.</param>
        /// <param name="canMasterstroke">True if the effect can be masterstroked, false otherwise.</param>
        /// <returns>Returns the maximum potential damage that the effect can do.</returns>
        public abstract int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true);
    }
}
