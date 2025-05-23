using System;
using System.Linq;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    ///     Effect that applies the configured status to the target fighter.
    /// </summary>
    [Serializable]
    public class ApplyStatusesEffect : AEffect
    {
        [SerializeReference] [Tooltip("The status to apply.")]
        public AStatus[] statusesToApply = { };

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
            foreach (AStatus status in statusesToApply)
            {
                receiver.ApplyStatus(status);
                Debug.Log($"Status {status.StatusType} applied to {receiver.name}.");

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
            if (!adjustGodFavorsPoints || initiator) return;
            if (atLeastOneBuff) initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.BUFF);
            if (atLeastOneDebuff) initiator.TryIncreaseGodFavorsPointsForAction(EGodFavorsAction.DEBUFF);
        }

        public override void RestoreEffect(Fighter receiver)
        {
            // Remove all the status with the configured types
            AStatus[] currentReceiverStatuses = receiver.StatusesManager.GetStatuses().Keys.ToArray();
            foreach (AStatus status in currentReceiverStatuses)
                if (statusesToApply.ToList().Contains(status))
                {
                    receiver.StatusesManager.RemoveStatus(status);
                    Debug.Log($"Status {status.StatusType} removed from {receiver.name}.");
                }
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            int totalPotentialDamage = 0;
            foreach (AStatus status in statusesToApply)
            {
                if (status.StatusType.IsBuff()) continue;
                totalPotentialDamage += status.GetPotentialDamages();
            }

            return totalPotentialDamage;
        }

        public override int GetPotentialEffectHeal(Fighter initiator, Fighter receiver, bool canMasterstroke)
        {
            int totalPotentialHeal = 0;
            foreach (AStatus status in statusesToApply)
            {
                if (!status.StatusType.IsBuff()) continue;
                totalPotentialHeal += status.GetPotentialHeal();
            }

            return totalPotentialHeal;
        }

        public override string GetUIEffectDescription()
        {
            string statusesListString =
                string.Join(", ", statusesToApply.Select(status => status.StatusType.ToUIString()));
            return $"Applies {statusesListString} to target.";
        }
    }
}