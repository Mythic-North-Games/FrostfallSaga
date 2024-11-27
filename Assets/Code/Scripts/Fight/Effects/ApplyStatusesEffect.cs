using System;
using System.Linq;
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
            bool isMasterstroke,
            Fighter initiator = null,
            bool adjustGodFavorsPoints = true
        )
        {
            bool atLeastOneBuff = false;
            bool atLeastOneDebuff = false;

            // Apply the statuses
            foreach (AStatus status in StatusesToApply)
            {
                receiver.ApplyStatus(status);
                Debug.Log($"Status {status.Name} applied to {receiver.name}.");

                if (status.StatusType.IsBuff())
                {
                    if (!atLeastOneBuff) atLeastOneBuff = true;
                }
                else
                {
                    if (!atLeastOneDebuff) atLeastOneDebuff = true;
                }
            }

            // Increase god favors points if enabled
            if (!adjustGodFavorsPoints || initiator != null) return;
            if (atLeastOneBuff) initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.BUFF);
            if (atLeastOneDebuff) initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.DEBUFF);
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Remove all the status with the configured types
            AStatus[] currentReceiverStatuses = receiver.StatusesManager.GetStatuses().Keys.ToArray();
            foreach (AStatus status in currentReceiverStatuses)
            {
                if (StatusesToApply.ToList().Contains(status))
                {
                    receiver.StatusesManager.RemoveStatus(status);
                    Debug.Log($"Status {status.Name} removed from {receiver.name}.");
                }
            }
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return 0;
        }
    }
}