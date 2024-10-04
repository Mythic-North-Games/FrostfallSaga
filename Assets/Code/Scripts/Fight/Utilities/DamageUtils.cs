using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Utilities
{
    public static class DamageUtils
    {
        public static bool TryDodge(Fighter attacker, Fighter defender)
        {
            float dodgeChance = Mathf.Clamp01((defender.Stats.dexterity - attacker.Stats.dexterity) / 100f);
            if (Random.value < dodgeChance)
            {
                Debug.Log($"{defender.name} dodged the attack from {attacker.name}!");
                return true; // Dodged the attack
            }
            return false; // Not dodged
        }

        public static int TryCriticalHit(Fighter attacker, int baseDamage)
        {
            bool isCritical = Random.value < attacker.Stats.criticalStrikeChance;
            if (isCritical)
            {
                // Random multiplier between 1.5 and 2.0 for critical hit damage
                float criticalMultiplier = 1.5f + (Random.value * 0.5f);
                int finalDamage = Mathf.RoundToInt(baseDamage * criticalMultiplier);
                Debug.Log($"Critical hit! Damage multiplied by {criticalMultiplier:F2}, final damage: {finalDamage}");
                return finalDamage; 
            }
            return baseDamage; 
        }
    }
}
