using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [Serializable]
    public abstract class AStatus
    {
        [field: SerializeField, Header("For the UI")] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite Icon { get; protected set; }

        [field: SerializeField, Header("Status parameters")] public EStatusType StatusType { get; protected set; }
        [field: SerializeField] public bool IsPermanent { get; protected set; }
        [field: SerializeField] public int Duration { get; protected set; }
        [field: SerializeField] public bool TriggerOnFirstApply { get; protected set; }
        [field: SerializeField] public bool IsRecurring { get; protected set; } = true;
        [field: SerializeField] public EStatusTriggerTime TriggerTime { get; protected set; } = EStatusTriggerTime.StartOfTurn;
        [field: SerializeField] public FighterBuffVisualsController VisualsController { get; protected set; }

        public AStatus() { }

        public AStatus(
            EStatusType statusType,
            string name,
            string description,
            bool isPermanent,
            int duration,
            bool triggerOnFirstApply,
            bool isRecurring,
            EStatusTriggerTime triggerTime,
            FighterBuffVisualsController visualsController
        )
        {
            StatusType = statusType;
            Name = name;
            Description = description;
            IsPermanent = isPermanent;
            Duration = duration;
            TriggerOnFirstApply = triggerOnFirstApply;
            IsRecurring = isRecurring;
            TriggerTime = triggerTime;
            VisualsController = visualsController;
        }

        /// <summary>
        /// Applies the status to the given fighter. Apply status logic, visuals, sounds and events are handled here.
        /// </summary>
        /// <param name="fighter">The fighter that will receive the status.</param>
        public virtual void ApplyStatus(Fighter fighter)
        {
            DoApplyStatus(fighter);
            fighter.onStatusApplied?.Invoke(fighter, this);

            if (VisualsController == null)
            {
                Debug.LogWarning("VisualsController is not set. No visuals will be shown for the status.");
                return;
            }

            VisualsController.ShowApplicationVisuals(fighter);
            if (!VisualsController.IsShowingRecurringVisuals)
            {
                VisualsController.ShowRecurringVisuals(fighter);
            }
        }

        /// <summary>
        /// Remove the status to the given fighter. Remove status logic, visuals, sounds and events are handled here.
        /// </summary>
        /// <param name="fighter">The fighter that will have the status removed.</param>
        public virtual void RemoveStatus(Fighter fighter)
        {
            DoRemoveStatus(fighter);
            fighter.onStatusRemoved?.Invoke(fighter, this);

            if (VisualsController == null)
            {
                return;
            }

            VisualsController.HideRecurringVisuals();
        }

        /// <summary>
        /// Executes the status logic. This method is called when the status is applied to a fighter.
        /// </summary>
        /// <param name="fighter">The fighter that will receive the status.</param>
        protected abstract void DoApplyStatus(Fighter fighter);

        /// <summary>
        /// Executes the status logic removal. This method is called when the status is removed from a fighter.
        /// </summary>
        /// <param name="fighter">The fighter that will have the status removed.</param>
        protected abstract void DoRemoveStatus(Fighter fighter);

#if UNITY_EDITOR
        public void SetDuration(int duration) => Duration = duration;
        public void SetIsRecurring(bool isRecurring) => IsRecurring = isRecurring;
        public void SetTriggerTime(EStatusTriggerTime triggerTime) => TriggerTime = triggerTime;
#endif
    }
}