using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    public abstract class Status : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Duration { get; private set; }
        [field: SerializeField] public bool IsRecurring { get; private set; } = true;
        [field: SerializeField] public EStatusTriggerTime TriggerTime { get; private set; } = EStatusTriggerTime.StartOfTurn;

        public abstract void ApplyStatus(Fighter fighter);
        public abstract void RemoveStatus(Fighter fighter);

        #if UNITY_EDITOR
        public void SetDuration(int duration) => Duration = duration;
        public void SetIsRecurring(bool isRecurring) => IsRecurring = isRecurring;
        public void SetTriggerTime(EStatusTriggerTime triggerTime) => TriggerTime = triggerTime;
        #endif
    }
}