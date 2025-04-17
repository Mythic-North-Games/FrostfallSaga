using System;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class WeaknessStatus : AStatus
    {
        [field: SerializeField] public int StrengthReduction { get; private set; }

        public WeaknessStatus()
        {
            StatusType = EStatusType.WEAKNESS;
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
            : base(EStatusType.WEAKNESS, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime,
                visualsController)
        {
            StrengthReduction = strengthReduction;
        }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, -StrengthReduction);
            Debug.Log(
                $"{fighter.name}'s strength is reduced by {StrengthReduction} == > Strength : {fighter.GetStrength()}.");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(
                EFighterMutableStat.Strength,
                StrengthReduction,
                false,
                false
            );
            Debug.Log($"{fighter.name}'s strength is back to normal !");
        }

        public override int GetPotentialDamages()
        {
            return 0;
        }

        public override int GetPotentialHeal()
        {
            return 0;
        }
    }
}