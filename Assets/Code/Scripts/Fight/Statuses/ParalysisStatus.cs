using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class ParalysisStatus : AStatus
    {
        private static readonly string NAME = "Paralysis";
        private static readonly string DESCRIPTION = "The fighter can't move.";

        public ParalysisStatus()
        {
            StatusType = EStatusType.PARALYSIS;
            Name = NAME;
            Description = DESCRIPTION;
        }

        public ParalysisStatus(
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController
        ) : base(EStatusType.PARALYSIS, NAME, DESCRIPTION, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
        {
        }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.SetIsParalyzed(true);
            Debug.Log($"{fighter.name} is paralyzed!");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.SetIsParalyzed(false);
            Debug.Log($"{fighter.name} is no longer paralyzed!");
        }
    }
}
