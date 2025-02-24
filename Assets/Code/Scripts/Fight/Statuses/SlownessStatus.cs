using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public class SlownessStatus : AStatus
    {
        private static readonly string NAME = "Slowness";
        private static readonly string DESCRIPTION = "The fighter's initiative is reduced.";

        [field: SerializeField] public int InitiativeReduction { get; private set; }

        public SlownessStatus()
        {
            StatusType = EStatusType.SLOWNESS;
            Name = NAME;
            Description = DESCRIPTION;
        }

        public SlownessStatus(
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController,
            int initiativeReduction
        )
            : base(EStatusType.SLOWNESS, NAME, DESCRIPTION, isPermanent, duration, triggerOnFirstApply, isRecurring, triggerTime, visualsController)
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
            fighter.UpdateMutableStat(
                EFighterMutableStat.Initiative,
                -InitiativeReduction,
                triggerAnimation: false,
                triggerEvent: false
            );
            Debug.Log($"{fighter.name}'s initiative is back to normal !");
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
