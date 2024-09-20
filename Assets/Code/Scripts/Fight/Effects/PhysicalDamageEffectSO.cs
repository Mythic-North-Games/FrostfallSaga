using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies physical damage to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicalDamageEffect", menuName = "ScriptableObjects/Fight/Effects/PhysicalDamageEffect", order = 0)]
    public class PhysicalDamageEffectSO : AEffectSO
    {
        [field: SerializeField, Range(0, 9999)] public int PhysicalDamageAmount { get; private set; }

        public override void ApplyEffect(Fighter fighter)
        {
            // No attacker involved, direct damage
            fighter.PhysicalWithstand(PhysicalDamageAmount);
        }

        public override void ApplyEffect(Fighter attacker, Fighter defender)
        {
            // Dodge logic using dexterity
            float dodgeChance = Mathf.Clamp01((defender.Stats.dexterity - attacker.Stats.dexterity) / 100f);
            if (Random.value < dodgeChance)
            {
                Debug.Log($"{defender.name} dodged the attack from {attacker.name}!");
                return;  // No damage applied if dodged
            }

            // Critical hit logic
            bool isCritical = Random.value < attacker.Stats.criticalStrikeChance;
            int finalDamage = PhysicalDamageAmount; // Base damage

            if (isCritical)
            {
                // Random multiplier between 1.5 and 2.0 for critical hit damage
                float criticalMultiplier = 1.5f + (Random.value * 0.5f);  // Random value from 1.5 to 2.0
                finalDamage = Mathf.RoundToInt(PhysicalDamageAmount * criticalMultiplier);  // Apply multiplier
                Debug.Log($"Critical hit! Damage multiplied by {criticalMultiplier:F2}, final damage: {finalDamage}");
            }

            // Apply final damage to defender
            defender.PhysicalWithstand(finalDamage);
        }
    }
}