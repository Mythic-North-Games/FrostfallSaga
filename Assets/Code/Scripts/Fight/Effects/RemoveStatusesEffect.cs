using System;
using System.Linq;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    ///     Effect that tries to remove the configured status from the target fighter.
    /// </summary>
    [Serializable]
    public class RemoveStatusesEffect : AEffect
    {
        [SerializeField] [Tooltip("Type of status the effect can remove from a fighter.")]
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
                if (RemovableStatusTypes.Contains(status.StatusType))
                {
                    receiver.RemoveStatus(status);
                    Debug.Log($"Status {status.StatusType} removed from {receiver.name}.");
                }

            // Increase god favors points if enabled
            if (adjustGodFavorsPoints && initiator != null)
                initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.HEAL);
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Remove statuses effect cannot be restored
        }

        public override int GetPotentialEffectDamages(Fighter _initiator, Fighter receiver, bool _canMasterstroke)
        {
            return 0;
        }

        public override int GetPotentialEffectHeal(Fighter _initiator, Fighter _receiver, bool _canMasterstroke)
        {
            return 0;
        }

        public override string GetUIEffectDescription()
        {
            string statusTypes = string.Join(", ", RemovableStatusTypes.Select(statusType => statusType.ToUIString()));
            return $"Removes {statusTypes} status from target.";
        }
    }
}