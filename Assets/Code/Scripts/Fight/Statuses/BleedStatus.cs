using System;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine;

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
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController,
            int bleedingDamage
        )
            : base(EStatusType.BLEED, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime,
                visualsController)
        {
            BleedingDamage = bleedingDamage;
        }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.ReceiveRawDamages(BleedingDamage, false);
            Debug.Log(
                $"{fighter.name} is bleeding and loses ${BleedingDamage} HP! ==> Health : ${fighter.GetHealth()}");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            Debug.Log($"{fighter.name} stopped bleeding.");
        }

        public override int GetPotentialDamages()
        {
            if (IsPermanent)
                return BleedingDamage * 3; // INFO: We multiply by three to get the "average" damage over the duration.
            return
                BleedingDamage * Duration / 2; // INFO: We divide by two to get the "average" damage over the duration.
        }

        public override int GetPotentialHeal()
        {
            return 0;
        }

        public override Sprite GetIcon()
        {
            return UIIconsProvider.Instance.GetIcon(UIIcons.BLEED.GetIconResourceName());
        }
    }
}