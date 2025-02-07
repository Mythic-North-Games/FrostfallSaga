using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells.Impediments;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;

namespace FrostfallSaga.Fight.FightCells
{
    /// <summary>
    /// A cell that can contain a fighter, an impediment and have applied multiple alterations.
    /// </summary>
    public class FightCell : Cell
    {
        [field: SerializeField, Header("Fight related"), Tooltip("The fighter occupying the ")]
        public Fighter Fighter { get; private set; }

        [field: SerializeField, Tooltip("The current obstacle or trap")] public AImpedimentSO Impediment { get; private set; }
        private GameObject _currentImpedimentGameObject;
        
        public FightCellAlterationsManager AlterationsManager { get; private set; }
        public Action onTrapTriggered;

        public FightCell()
        {
            AlterationsManager = new(this);
        }

        public void SetFighter(Fighter fighter)
        {
            Fighter = fighter;
        }
           public Fighter GetFighter()
        {
            return Fighter;
        }

        public void SetImpediment(AImpedimentSO impediment, GameObject impediementGameObject)
        {
            Impediment = impediment;
            _currentImpedimentGameObject = impediementGameObject;
        }

        /// <summary>
        /// Trigger the trap if there is one on the cell, if a fighter is present and the trigger time is the same as the one given.
        /// Listen to the onTrapTriggered event to know when the trap has been triggered (animation done...).
        /// </summary>
        /// <param name="triggerTime">The trap trigger time.</param>
        public void TriggerTrapIfAny(ETrapTriggerTime triggerTime)
        {
            if (Impediment is TrapSO trap && HasFighter() && trap.TriggerTimes.Contains(triggerTime))
            {
                trap.onTrapTriggered += () => onTrapTriggered?.Invoke();
                trap.Trigger(Fighter, this);
            }
            else
            {
                onTrapTriggered?.Invoke();
            }
        }

        /// <summary>
        /// Update the alterations on the cell.
        /// If a temporary alteration is over, it will be removed.
        /// If a trap triggering on stay is present and a fighter occupies the cell, the trap will be triggered.
        /// </summary>
        public void UpdateAlterations()
        {
            if (Impediment is TrapSO trap && HasFighter() && trap.TriggerTimes.Contains(ETrapTriggerTime.OnStay))
            {
                trap.Trigger(Fighter, this);
            }
            AlterationsManager.UpdateAlterations();
        }

        public AFightCellAlteration[] GetAlterations()
        {
            return AlterationsManager.GetAlterations();
        }

        public bool HasFighter()
        {
            return Fighter != null;
        }

        public bool HasObstacle()
        {
            return Impediment != null && Impediment is ObstacleSO;
        }

        public bool HasTrap()
        {
            return Impediment != null && Impediment is TrapSO;
        }

        public override bool IsTerrainAccessible()
        {
            return base.IsTerrainAccessible() && !HasObstacle();
        }

        public override bool IsFree()
        {
            return IsTerrainAccessible() && !HasFighter();
        }

        public GameObject GetImpedimentGameObject()
        {
            return _currentImpedimentGameObject;
        }
    }
}
