using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;

namespace FrostfallSaga.Fight.FightCells
{
    public class FightCellAlterationsManager
    {
        private readonly List<AFightCellAlteration> _activePermanentAlterations;
        private readonly Dictionary<AFightCellAlteration, int> _activeTemporaryAlterations;

        private readonly FightCell _fightCell;
        private AFightCellAlteration _nextAlterationToAdd;
        public Action<AFightCellAlteration[]> onAlterationsUpdated;

        public FightCellAlterationsManager(FightCell fightCell)
        {
            _fightCell = fightCell;
            _activePermanentAlterations = new List<AFightCellAlteration>();
            _activeTemporaryAlterations = new Dictionary<AFightCellAlteration, int>();
        }

        public AFightCellAlteration[] GetAlterations()
        {
            return _activePermanentAlterations.Concat(_activeTemporaryAlterations.Keys).ToArray();
        }

        public void ApplyNewAlteration(AFightCellAlteration newAlteration)
        {
            if (!newAlteration.CanApplyWithFighter && _fightCell.HasFighter())
                throw new FightCellAlterationApplicationException(
                    "The alteration cannot be applied to a cell with a fighter."
                );

            _nextAlterationToAdd = newAlteration;
            _nextAlterationToAdd.onAlterationApplied += OnNewAlterationApplied;

            AFightCellAlteration currentAlterationOfType = GetFightCellAlterationOfSameType(newAlteration);
            if (currentAlterationOfType != null)
            {
                if (!currentAlterationOfType.CanBeReplaced)
                    throw new FightCellAlterationApplicationException(
                        $"There is already an alteration of type {newAlteration.GetType().Name} on the cell."
                    );

                currentAlterationOfType.onAlterationRemoved += OnAlterationToBeReplacedRemoved;
                currentAlterationOfType.Remove(_fightCell);
                return;
            }

            _nextAlterationToAdd.Apply(_fightCell);
        }

        public void UpdateAlterations()
        {
            List<AFightCellAlteration> toRemove = new();
            foreach (AFightCellAlteration alteration in _activeTemporaryAlterations.Keys.ToList())
            {
                _activeTemporaryAlterations[alteration]--;
                if (_activeTemporaryAlterations[alteration] == 0) toRemove.Add(alteration);
            }

            foreach (AFightCellAlteration alterationToRemove in toRemove)
            {
                alterationToRemove.onAlterationRemoved += OnAlterationRemoved;
                alterationToRemove.Remove(_fightCell);
            }
        }

        private void OnNewAlterationApplied(FightCell fightCell, AFightCellAlteration apppliedAlteration)
        {
            // Stop listening to the event
            apppliedAlteration.onAlterationApplied -= OnNewAlterationApplied;

            // Add to active list
            if (apppliedAlteration.IsPermanent)
                _activePermanentAlterations.Add(apppliedAlteration);
            else
                _activeTemporaryAlterations.Add(apppliedAlteration, apppliedAlteration.Duration);

            // Trigger the updated event
            onAlterationsUpdated?.Invoke(GetAlterations());
        }

        private void OnAlterationToBeReplacedRemoved(FightCell fightCell, AFightCellAlteration alteration)
        {
            // Stop listening to the event
            alteration.onAlterationRemoved -= OnAlterationToBeReplacedRemoved;

            // Remove from active list
            _activePermanentAlterations.Remove(alteration);
            _activeTemporaryAlterations.Remove(alteration);

            // Now, apply the new alteration
            _nextAlterationToAdd.Apply(_fightCell);
            _nextAlterationToAdd = null;
        }

        private void OnAlterationRemoved(FightCell fightCell, AFightCellAlteration removedAlteration)
        {
            // Stop listening to the event
            removedAlteration.onAlterationRemoved -= OnAlterationRemoved;

            // Remove from active list
            _activePermanentAlterations.Remove(removedAlteration);
            _activeTemporaryAlterations.Remove(removedAlteration);

            // Trigger the updated event
            onAlterationsUpdated?.Invoke(GetAlterations());
        }

        private AFightCellAlteration GetFightCellAlterationOfSameType(AFightCellAlteration alteration)
        {
            return _activePermanentAlterations.FirstOrDefault(alteration.GetType().IsInstanceOfType) ??
                   _activeTemporaryAlterations.Keys.FirstOrDefault(alteration.GetType().IsInstanceOfType);
        }
    }
}