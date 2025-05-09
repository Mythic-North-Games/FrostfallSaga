using System;
using System.Linq;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Fight.FightCells.Impediments;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells
{
    /// <summary>
    ///     A cell that can contain a fighter, an impediment and have applied multiple alterations.
    /// </summary>
    public class FightCell : Cell
    {
        [field: SerializeField]
        [field: Header("Fight related")]
        [field: Tooltip("The optional fighter occupying the cell.")]
        public Fighter Fighter { get; private set; }

        [field: SerializeField]
        [field: Tooltip("The current obstacle or trap")]
        public AImpedimentSO Impediment { get; private set; }

        private GameObject _currentImpedimentGameObject;
        public Action onTrapTriggered;

        public FightCell()
        {
            AlterationsManager = new FightCellAlterationsManager(this);
        }

        public FightCellAlterationsManager AlterationsManager { get; }

        public void SetFighter(Fighter fighter)
        {
            Fighter = fighter;
        }

        public void SetImpediment(AImpedimentSO impediment, GameObject impediementGameObject)
        {
            Impediment = impediment;
            _currentImpedimentGameObject = impediementGameObject;
        }

        /// <summary>
        ///     Trigger the trap if there is one on the cell, if a fighter is present and the trigger time is the same as the one
        ///     given.
        ///     Listen to the onTrapTriggered event to know when the trap has been triggered (animation done...).
        /// </summary>
        /// <param name="triggerTime">The trap trigger time.</param>
        public void TriggerTrapIfAny(ETrapTriggerTime triggerTime)
        {
            if (Impediment is TrapSO trap && HasFighter() && trap.TriggerTimes.Contains(triggerTime))
            {
                trap.onTrapTriggered += () => onTrapTriggered?.Invoke();
                trap.Trigger(Fighter);
            }
            else
            {
                onTrapTriggered?.Invoke();
            }
        }

        /// <summary>
        ///     Update the alterations on the cell.
        ///     If a temporary alteration is over, it will be removed.
        ///     If a trap triggering on stay is present and a fighter occupies the cell, the trap will be triggered.
        /// </summary>
        public void UpdateAlterations()
        {
            if (Impediment is TrapSO trap && HasFighter() && trap.TriggerTimes.Contains(ETrapTriggerTime.OnStay))
                trap.Trigger(Fighter);
            AlterationsManager.UpdateAlterations();
        }

        public AFightCellAlteration[] GetAlterations()
        {
            return AlterationsManager.GetAlterations();
        }

        public override bool IsTerrainAccessible()
        {
            return IsAccessible && !HasObstacle();
        }

        public GameObject GetImpedimentGameObject()
        {
            return _currentImpedimentGameObject;
        }

        public override bool IsFree()
        {
            return IsAccessible && !HasFighter() && !HasObstacle();
        }
        
        public bool HasFighter()
        {
            return Fighter;
        }

        private bool HasObstacle()
        {
            return Impediment is ObstacleSO;
        }

        private bool HasTrap()
        {
            return Impediment is TrapSO;
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "FightCell:\n" +
                   $"- Fighter: {(Fighter != null ? Fighter.name : "None")}\n" +
                   $"- Impediment: {(Impediment != null ? Impediment.name : "None")}\n" +
                   $"- HasObstacle: {HasObstacle()}\n" +
                   $"- HasTrap: {HasTrap()}\n" +
                   $"- Alterations: {(GetAlterations().Length > 0 ? string.Join(", ", GetAlterations().Select(a => a.Name)) : "None")}";
        }
    }
}