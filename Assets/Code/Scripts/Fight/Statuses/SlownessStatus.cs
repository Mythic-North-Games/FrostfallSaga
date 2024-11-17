using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class SlownessStatus : AStatus
    {
        [field: SerializeField] public int InitiativeReduction { get; private set; }

        public SlownessStatus()
        {
            StatusType = EStatusType.SLOWNESS;
        }

        public SlownessStatus(
            string name,
            string description,
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController,
            int initiativeReduction
        )
            : base(EStatusType.SLOWNESS, name, description, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
        {
            InitiativeReduction = initiativeReduction;
        }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, InitiativeReduction);
            Debug.Log($"{fighter.name}'s initiative is reduced by {InitiativeReduction}.");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, -InitiativeReduction, false);
            Debug.Log($"{fighter.name}'s initiative is back to normal !");
        }
    }
}
