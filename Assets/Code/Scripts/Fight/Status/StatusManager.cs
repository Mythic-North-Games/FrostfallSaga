using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    public class StatusesManager
    {
        private readonly Fighter _fighter;
        private Dictionary<Status, (bool isActive, int duration)> _statuses = new();

        public StatusesManager(Fighter fighter)
        {
            _fighter = fighter;
        }

        public void ApplyStatus(Status status)
        {
            _statuses[status] = (true, status.Duration);
        }

        public void UpdateStatuses(EStatusTriggerTime triggerTime)
        {
            List<Status> statusesToRemove = new();
            Dictionary<Status, (bool isActive, int duration)> tempStatus = new();

            foreach (var status in _statuses)
            {
                tempStatus[status.Key] = (status.Value.isActive, status.Value.duration);
            }
            foreach (var status in _statuses.Keys)
            {
                var (isActive, duration) = tempStatus[status];
                duration--;
                if (isActive)
                {
                    if (triggerTime == status.TriggerTime){
                        status.ApplyStatus(_fighter);
                        if (!status.IsRecurring) isActive = false;
                    } else
                    {
                        duration++;
                    }
                }
                tempStatus[status] = (isActive, duration);
                if (duration <= 0)
                {
                    statusesToRemove.Add(status);
                }

            }

            _statuses = tempStatus;
            foreach (var status in statusesToRemove)
            {
                RemoveStatus(status);
                status.RemoveStatus(_fighter);

            }
        }

        public void RemoveStatus(Status status)
        {
            if (_statuses.ContainsKey(status))
            {
                _statuses.Remove(status);
            }
        }

        public Dictionary<Status, (bool isActive, int duration)> GetStatusEffects()
        {
            return _statuses;
        }
    }
}