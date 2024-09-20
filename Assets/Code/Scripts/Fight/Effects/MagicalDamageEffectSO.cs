using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;

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

        public override void ApplyEffect(Fighter fighter)
        {
            fighter.MagicalWithstand(MagicalDamageAmount, MagicalElement);
        }

        public override void ApplyEffect(Fighter attacker, Fighter defender)
        {
            // Dodge logic using dexterity
            float dodgeChance = Mathf.Clamp01((defender.Stats.dexterity - attacker.Stats.dexterity) / 100f);
            if (Random.value < dodgeChance)
            {
                Debug.Log($"{defender.name} dodged the magical attack from {attacker.name}!");
                return;  // No damage if dodged
            }

            // Critical hit logic
            bool isCritical = Random.value < attacker.Stats.criticalStrikeChance;
            int finalDamage = MagicalDamageAmount;

            if (isCritical)
            {
                // Random multiplier between 1.5 and 2.0 for critical hit damage
                float criticalMultiplier = 1.5f + (Random.value * 0.5f); 
                finalDamage = Mathf.RoundToInt(MagicalDamageAmount * criticalMultiplier);
                Debug.Log($"Critical magical hit! Damage multiplied by {criticalMultiplier:F2}, final damage: {finalDamage}");
            }
            defender.MagicalWithstand(finalDamage, MagicalElement);
        }
    }
}
