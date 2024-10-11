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

        public override void ApplyEffect(Fighter initiator, Fighter target, bool canMasterstroke = true, bool canDodge = true)
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(target))
            {
                Debug.Log($"{target.name} dodged heal effect.");
                return;
            }

            int finalDamageAmount = PhysicalDamageAmount;

            // Calculate masterstroke
            if (canMasterstroke)
            {
                finalDamageAmount = TryMasterstroke(initiator, PhysicalDamageAmount);
                if (finalDamageAmount != PhysicalDamageAmount)
                {
                    Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
                }
            }

            target.PhysicalWithstand(finalDamageAmount);
        }
    }
}