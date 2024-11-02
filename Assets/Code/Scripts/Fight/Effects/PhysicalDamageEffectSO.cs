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

        public override void ApplyEffect(Fighter initiator, Fighter receiver, bool canMasterstroke = true, bool canDodge = true)
        {
            // Try dodge if enabled
            if (canDodge && TryDodge(receiver))
            {
                Debug.Log($"{receiver.name} dodged heal effect.");
                return;
            }

            int finalDamageAmount = PhysicalDamageAmount;
            bool masterstrokeSucceeded = false;

            // Calculate masterstroke
            if (canMasterstroke)
            {
                finalDamageAmount = TryMasterstroke(initiator, PhysicalDamageAmount);
                masterstrokeSucceeded = finalDamageAmount != PhysicalDamageAmount;
                if (masterstrokeSucceeded)
                {
                    Debug.Log($"Masterstroke succeeded, damage amount increased to {finalDamageAmount}.");
                }
            }

            receiver.PhysicalWithstand(finalDamageAmount);
            receiver.onEffectReceived?.Invoke(receiver, initiator, this, masterstrokeSucceeded);
            Debug.Log($"Dealt {finalDamageAmount} physical damage to {receiver.name}.");
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return PhysicalDamageAmount * (canMasterstroke ? 2 : 1);
        }
    }
}