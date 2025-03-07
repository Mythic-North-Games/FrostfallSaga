using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    public class StatusesManager
    {
        private readonly Fighter _fighter;
        private readonly List<AStatus> _permanentStatuses = new();
        private readonly Dictionary<AStatus, (bool isActive, int duration)> _temporaryStatuses = new();

        public StatusesManager(Fighter fighter)
        {
            _fighter = fighter;
        }

        public void ApplyStatus(AStatus status)
        {
            _permanentStatuses.Find(s => s.GetType() == status.GetType())?.RemoveStatus(_fighter);
            _temporaryStatuses.Keys.ToList().Find(s => s.GetType() == status.GetType())?.RemoveStatus(_fighter);

            if (status.IsPermanent)
            {
                _permanentStatuses.Add(status);
                _fighter.onStatusApplied?.Invoke(_fighter, status);
            }
            else
            {
                _temporaryStatuses[status] = (true, status.Duration);
                _fighter.onStatusApplied?.Invoke(_fighter, status);
            }

            if (status.TriggerOnFirstApply)
            {
                status.ApplyStatus(_fighter);
                if (!status.IsRecurring && !status.IsPermanent) _temporaryStatuses[status] = (false, status.Duration);
            }
        }

        public void UpdateStatuses(EStatusTriggerTime triggerTime)
        {
            List<AStatus> temporaryStatusesOfTriggerTime =
                _temporaryStatuses.Keys.Where(s => s.TriggerTime == triggerTime).ToList();

            foreach (AStatus status in temporaryStatusesOfTriggerTime)
            {
                (bool isActuallyActive, int currentDuration) = _temporaryStatuses[status];
                bool willBeActive = isActuallyActive;

                if (isActuallyActive)
                {
                    status.ApplyStatus(_fighter);
                    if (!status.IsRecurring) willBeActive = false;
                }

                if (currentDuration == 1) // Last turn of the status
                    RemoveStatus(status);
                else
                    _temporaryStatuses[status] = (willBeActive, currentDuration - 1);
            }

            _permanentStatuses
                .Where(status => status.TriggerTime == triggerTime && status.IsRecurring)
                .ToList()
                .ForEach(status => status.ApplyStatus(_fighter));
        }

        public void RemoveStatus(AStatus statusToRemove)
        {
            if (_temporaryStatuses.Keys.Contains(statusToRemove))
            {
                statusToRemove.RemoveStatus(_fighter);
                _temporaryStatuses.Remove(statusToRemove);
                _fighter.onStatusRemoved?.Invoke(_fighter, statusToRemove);
            }
        }

        /// <summary>
        ///     Returns a dictionary of all statuses that are currently active on the fighter.
        /// </summary>
        /// <returns></returns>
        public Dictionary<AStatus, (bool isActive, int duration)> GetStatuses()
        {
            Dictionary<AStatus, (bool isActive, int duration)> statuses = new();

            foreach (AStatus status in _permanentStatuses) statuses[status] = (true, -1);

            foreach (KeyValuePair<AStatus, (bool isActive, int duration)> status in _temporaryStatuses)
                statuses[status.Key] = status.Value;

            return statuses;
        }
    }
}