using UnityEngine;
using FrostfallSaga.Fight.Fighters;

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

        public override void ApplyEffect(Fighter initator, Fighter defender, bool canMasterstroke = true, bool canDodge = true)
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(defender))
            {
                Debug.Log($"{defender.name} dodged heal effect.");
                return;
            }

            int finalDamageAmount = MagicalDamageAmount;

            // Calculate masterstroke
            if (canMasterstroke)
            {
                finalDamageAmount = TryMasterstroke(initator, MagicalDamageAmount);
                if (finalDamageAmount != MagicalDamageAmount)
                {
                    Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
                }
            }

            defender.MagicalWithstand(finalDamageAmount, MagicalElement);
        }
    }
}