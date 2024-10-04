using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Utilities;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies physical damage to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicalDamageEffect", menuName = "ScriptableObjects/Fight/Effects/PhysicalDamageEffect", order = 0)]
    public class PhysicalDamageEffectSO : AEffectSO
    {
        [field: SerializeField, Range(0, 9999)] public int PhysicalDamageAmount { get; private set; }

        // Main ApplyEffect method (required override)
        public override void ApplyEffect(Fighter attacker, Fighter defender)
        {
            ApplyEffect(attacker, defender, true, true);  // Call the overloaded method with default values
        }

        // Overloaded ApplyEffect with optional parameters for canCrit and canDodge
        public void ApplyEffect(Fighter attacker, Fighter defender, bool canCrit = true, bool canDodge = true)
        {
            // Dodge logic if enabled
            if (canDodge && DamageUtils.TryDodge(attacker, defender)) return;

            // Critical hit logic if enabled
            int finalDamage = PhysicalDamageAmount;
            if (canCrit)
            {
                finalDamage = DamageUtils.TryCriticalHit(attacker, PhysicalDamageAmount);
            }

            // Apply physical damage
            defender.PhysicalWithstand(finalDamage);
        }
    }
}
