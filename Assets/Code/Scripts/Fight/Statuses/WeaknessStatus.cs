using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class WeaknessStatus : AStatus
    {
        private static readonly string NAME = "Weakness";
        private static readonly string DESCRIPTION = "The fighter's strength is reduced.";

        [field: SerializeField] public int StrengthReduction { get; private set; }

        public WeaknessStatus()
        {
            StatusType = EStatusType.WEAKNESS;
            Name = NAME;
            Description = DESCRIPTION;
        }

        public WeaknessStatus(
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController,
            int strengthReduction
        )
            : base(EStatusType.WEAKNESS, NAME, DESCRIPTION, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
        {
            StrengthReduction = strengthReduction;
        }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, -StrengthReduction);
            Debug.Log($"{fighter.name}'s strength is reduced by {StrengthReduction} == > Strength : {fighter.GetStrength()}.");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, StrengthReduction, false);
            Debug.Log($"{fighter.name}'s strength is back to normal !");
        }
    }
}
