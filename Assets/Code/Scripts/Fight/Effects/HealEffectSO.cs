using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "HealEffect", menuName = "ScriptableObjects/Fight/Effects/HealEffect", order = 0)]
    public class HealEffectSO : AEffectSO
    {
        [field: SerializeField, Range(0, 9999)] public int HealAmount { get; private set; }

        public override void ApplyEffect(Fighter attacker, Fighter defender, bool canMasterstroke = true, bool canDodge = true)
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(defender))
            {
                Debug.Log($"{defender.name} dodged heal effect.");
                return;
            }

            int finalHealAmount = HealAmount;

            // Calculate masterstroke heal
            if (canMasterstroke)
            {
                finalHealAmount = TryMasterstroke(attacker, HealAmount);
                if (finalHealAmount != HealAmount)
                {
                    Debug.Log($"Masterstroke succeeded, heal amount increased to {finalHealAmount}.");
                }
            }

            // Apply the heal
            defender.Heal(finalHealAmount);
            Debug.Log($"Healed {defender.name} for {finalHealAmount} health.");
        }
    }
}