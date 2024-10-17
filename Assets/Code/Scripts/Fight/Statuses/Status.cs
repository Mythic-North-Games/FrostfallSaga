using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    public abstract class Status : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Duration { get; private set; }
        [field: SerializeField] public bool TriggerOnFirstApply { get; private set; }
        [field: SerializeField] public bool IsRecurring { get; private set; } = true;
        [field: SerializeField] public EStatusTriggerTime TriggerTime { get; private set; } = EStatusTriggerTime.StartOfTurn;
        [field: SerializeField] public StatusVisualsController VisualsController { get; private set; }

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

            VisualsController.ShowStatusApplicationVisuals(fighter);
            if (!VisualsController.IsShowingRecurringVisuals)
            {
                VisualsController.ShowRecurringStatusVisuals(fighter);
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

            VisualsController.HideRecurringStatusVisuals();
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