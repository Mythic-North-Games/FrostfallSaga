using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class BleedStatus : AStatus
    {
        private static readonly string NAME = "Bleed";
        private static readonly string DESCRIPTION = "The fighter's health is reduced.";

        [field: SerializeField] public int BleedingDamage { get; private set; }

        public BleedStatus()
        {
            StatusType = EStatusType.BLEED;
            Name = NAME;
            Description = DESCRIPTION;
        }

        public BleedStatus(
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController,
            int bleedingDamage
        )
            : base(EStatusType.BLEED, NAME, DESCRIPTION, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
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
