using System.Linq;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that tries to remove the configured status from the target fighter.
    /// </summary>
    [CreateAssetMenu(
        fileName = "RemoveStatusEffect",
        menuName = "ScriptableObjects/Fight/Effects/RemoveStatusEffect",
        order = 0
    )]
    public class RemoveStatusesEffectSO : AEffectSO
    {
        [field: SerializeField, Tooltip("Type of status the effect can remove from a fighter.")]
        public EStatusType[] RemovableStatusTypes { get; private set; } = { };

        public override void ApplyEffect(
            Fighter receiver,
            Fighter initiator = null,
            bool canMasterstroke = true,
            bool canDodge = true
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
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return 0;
        }
    }
}