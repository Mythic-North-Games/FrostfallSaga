using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Base class for all effects that abilities will have.
    /// </summary>
    public abstract class AEffectSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public bool Dodgable { get; private set; } = true;
        [field: SerializeField] public bool Masterstrokable { get; private set; } = true;

        /// <summary>
        /// Apply the effect to the target fighter using the initator stats for possible masterstroke and dodge chance.
        /// </summary>
        /// <param name="initiator">The effect initiator.</param>
        /// <param name="receiver">The fighter that will receive the effect.</param>
        /// <param name="canMasterstroke">True if the effect can be masterstroked, false otherwise.</param>
        /// <param name="canDodge">True if the effect can be dodged, false otherwise.</param>
        public abstract void ApplyEffect(Fighter initiator, Fighter receiver, bool canMasterstroke = true, bool canDodge = true);

        /// <summary>
        /// Make the given fighter try to dodge something.
        /// </summary>
        /// <param name="defender">The fighter that tries to dodge.</param>
        /// <returns>True if the dodge is successfull, false otherwise</returns>
        public static bool TryDodge(Fighter defender)
        {
            bool isDodge = Random.value < defender.GetDodgeChance();
            if (isDodge)
            {
                return true; // Dodged the attack
            }
            return false; // Not dodged
        }

        /// <summary>
        /// Make the given fighter try to masterstroke a given value.
        /// </summary>
        /// <param name="attacker">The fighter that tries to masterstroke.</param>
        /// <param name="baseValue">The base value before the potential masterstroke.</param>
        /// <returns>Either the base value if masterstroke failed or the augmented value.</returns>
        public static int TryMasterstroke(Fighter attacker, int baseValue)
        {
            bool isMasterstroke = Random.value < attacker.GetMasterstrokeChance();
            if (isMasterstroke)
            {
                // Random multiplier between 1.5 and 2.0 for critical hit damage
                float criticalMultiplier = 1.5f + (Random.value * 0.5f);
                int finalValue = Mathf.RoundToInt(baseValue * criticalMultiplier);
                Debug.Log($"Masterstroke! Damage multiplied by {criticalMultiplier:F2}, final damage: {finalValue}");
                return finalValue;
            }
            return baseValue;
        }
    }
}
