using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that tries to remove the configured status from the target fighter.
    /// </summary>
    [Serializable]
    public class RemoveStatusesEffect : AEffect
    {
        [SerializeField, Tooltip("Type of status the effect can remove from a fighter.")]
        public EStatusType[] RemovableStatusTypes = { };

        public override void ApplyEffect(
            Fighter receiver,
            bool isMasterstroke,
            Fighter initiator = null,
            bool adjustGodFavorsPoints = true
        )
        {
            // Remove all the status with the configured types
            AStatus[] currentReceiverStatuses = receiver.StatusesManager.GetStatuses().Keys.ToArray();
            foreach (AStatus status in currentReceiverStatuses)
            {
                if (RemovableStatusTypes.Contains(status.StatusType))
                {
                    receiver.StatusesManager.RemoveStatus(status);
                    Debug.Log($"Status {status.Name} removed from {receiver.name}.");
                }
            }
            receiver.onEffectReceived?.Invoke(receiver, initiator, this, false);

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints && initiator != null)
            {
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.HEAL);
            }
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Remove statuses effect cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            return 0;
        }
    }
}