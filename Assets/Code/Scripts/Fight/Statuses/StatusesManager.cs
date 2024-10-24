using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    public class StatusesManager
    {
        private readonly Fighter _fighter;
        private readonly Dictionary<AStatus, (bool isActive, int duration)> _statuses = new();

        public StatusesManager(Fighter fighter)
        {
            _fighter = fighter;
        }

        public void ApplyStatus(AStatus status)
        {
            _statuses[status] = (true, status.Duration);
            if (status.TriggerOnFirstApply)
            {
                status.ApplyStatus(_fighter);
                if (!status.IsRecurring) _statuses[status] = (false, status.Duration);
            }
        }

        public void UpdateStatuses(EStatusTriggerTime triggerTime)
        {
            List<AStatus> statusesOfTriggerTime = _statuses.Keys.Where(s => s.TriggerTime == triggerTime).ToList();

            foreach (AStatus status in statusesOfTriggerTime)
            {
                var (isActuallyActive, currentDuration) = _statuses[status];
                bool willBeActive = isActuallyActive;

                if (isActuallyActive)
                {
                    status.ApplyStatus(_fighter);
                    if (!status.IsRecurring) willBeActive = false;
                }

                if (currentDuration == 1)   // Last turn of the status
                {
                    RemoveStatus(status);
                }
                else
                {
                    _statuses[status] = (willBeActive, currentDuration - 1);
                }
            }
        }

        public void RemoveStatus(AStatus statusToRemove)
        {
            if (_statuses.Keys.Contains(statusToRemove))
            {
                statusToRemove.RemoveStatus(_fighter);
                _statuses.Remove(statusToRemove);
            }
        }

        public Dictionary<AStatus, (bool isActive, int duration)> GetStatuses()
        {
            return _statuses;
        }
    }
}