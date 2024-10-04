using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Utilities;

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

        // Main ApplyEffect method (required override)
        public override void ApplyEffect(Fighter attacker, Fighter defender)
        {
            ApplyEffect(attacker, defender, true, true);
        }
        public void ApplyEffect(Fighter attacker, Fighter defender, bool canCrit = true, bool canDodge = true)
        {
            if (canDodge && DamageUtils.TryDodge(attacker, defender)) return;
            int finalDamage = MagicalDamageAmount;
            if (canCrit)
            {
                finalDamage = DamageUtils.TryCriticalHit(attacker, MagicalDamageAmount);
            }
            defender.MagicalWithstand(finalDamage, MagicalElement);
        }
    }
}
