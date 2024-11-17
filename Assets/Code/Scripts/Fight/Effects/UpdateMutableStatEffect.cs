using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [Serializable]
    public class UpdateMutableStatEffect : AEffect
    {
        [field: SerializeField] public EFighterMutableStat StatToUpdate;
        [field: SerializeField] public int Amount;
        [field: SerializeField] public bool UsePercentage;

        public UpdateMutableStatEffect()
        {
            Masterstrokable = false;
            Dodgable = false;
        }

        public override void ApplyEffect(
            Fighter receiver,
            Fighter initiator = null,
            bool canMasterstroke = true,
            bool canDodge = true
        )
        {
            float finalUpdateAmount = Amount;
            if (UsePercentage)
            {
                finalUpdateAmount = receiver.GetMutableStat(StatToUpdate) * Amount / 100f;
            }

            receiver.UpdateMutableStat(StatToUpdate, finalUpdateAmount);
            receiver.onEffectReceived?.Invoke(receiver, initiator, this, false);
            Debug.Log($"{receiver.name} {StatToUpdate} updated by {finalUpdateAmount}.");
        }

        public override void RestoreEffect(Fighter receiver)
        {
            int finalUpdateAmount = Amount;
            if (UsePercentage)
            {
                finalUpdateAmount = (int)(receiver.GetMutableStat(StatToUpdate) * Amount / 100f);
            }

            receiver.UpdateMutableStat(StatToUpdate, -finalUpdateAmount);
        }

        public override int GetPotentialEffectDamages(Fighter initiator, Fighter receiver, bool canMasterstroke = true)
        {
            return 0;
        }
    }
}