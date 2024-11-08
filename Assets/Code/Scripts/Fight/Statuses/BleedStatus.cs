using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class BleedStatus : AStatus
    {
        [field: SerializeField] public int BleedingDamage { get; private set; }

        public BleedStatus()
        {
            StatusType = EStatusType.BLEED;
        }

        public BleedStatus(
            string name,
            string description,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            StatusVisualsController visualsController,
            int bleedingDamage
        )
            : base(EStatusType.BLEED, name, description, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
        {
            BleedingDamage = bleedingDamage;
        }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.ReceiveRawDamages(BleedingDamage);
            Debug.Log($"{fighter.name} is bleeding and loses ${BleedingDamage} HP! ==> Health : ${fighter.GetHealth()}");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            Debug.Log($"{fighter.name} stopped bleeding.");
        }
    }
}
