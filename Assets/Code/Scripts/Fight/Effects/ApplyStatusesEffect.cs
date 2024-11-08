using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies the configured status to the target fighter.
    /// </summary>
    [Serializable]
    public class ApplyStatusesEffect : AEffect
    {
        [SerializeReference, Tooltip("The status to apply.")] public AStatus[] StatusesToApply = { };

        public override void ApplyEffect(
            Fighter receiver,
            Fighter initiator = null,
            bool canMasterstroke = true,
            bool canDodge = true
        )
        {
            foreach (AStatus status in StatusesToApply)
            {
                receiver.ApplyStatus(status);
                Debug.Log($"Status {status.Name} applied to {receiver.name}.");
            }
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return 0;
        }
    }
}