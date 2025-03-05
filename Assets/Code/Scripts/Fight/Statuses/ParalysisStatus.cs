using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class ParalysisStatus : AStatus
    {
        public ParalysisStatus()
        {
            StatusType = EStatusType.PARALYSIS;
        }

        public ParalysisStatus(
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController
        ) : base(EStatusType.PARALYSIS, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
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
