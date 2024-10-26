using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies the configured status to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "RemoveStatusEffect", menuName = "ScriptableObjects/Fight/Effects/RemoveStatusEffect", order = 0)]
    public class ApplyStatusesEffectSO : AEffectSO
    {
        [field: SerializeField, Tooltip("The status to apply.")] public AStatus[] StatusesToApply { get; private set; } = { };

        public override void ApplyEffect(Fighter initiator, Fighter receiver, bool canMasterstroke = false, bool canDodge = false)
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