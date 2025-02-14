using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells.Impediments;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using System.Collections.Generic;
using System.Collections;

namespace FrostfallSaga.Fight.FightCells
{
    /// <summary>
    /// A cell that can contain a fighter, an impediment and have applied multiple alterations.
    /// </summary>
    public class FightCell : Cell
    {
        [field: SerializeField, Header("Fight related"), Tooltip("The fighter occupying the ")]
        public Fighter Fighter { get; private set; }

        [field: SerializeField, Tooltip("The current obstacle")] public AImpedimentSO Obstacle { get; private set; }
        [field: SerializeField, Tooltip("The current traps")] public List<TrapSO> TrapList { get; private set; }
        private Queue<TrapSO> _trapsToTrigger = new Queue<TrapSO>();
        private GameObject _currentImpedimentGameObject;
        public FightCellAlterationsManager AlterationsManager { get; private set; }
        public Action onTrapTriggered;

    
        public FightCell()
        {
            AlterationsManager = new(this);
            TrapList = new List<TrapSO>();
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

            if (impediment is TrapSO impdementTrap)
            {
                bool alreadyExists = TrapList.Any(trap => trap.TrapType == impdementTrap.TrapType);
                if (!alreadyExists)
                {
                    TrapList.Add(impdementTrap);
                }
            }
            else
            {
                Obstacle = impediment;
            }
            _currentImpedimentGameObject = impediementGameObject;
        }


        /// <summary>
        /// Trigger the trap if there is one on the cell, if a fighter is present and the trigger time is the same as the one given.
        /// Listen to the onTrapTriggered event to know when the trap has been triggered (animation done...).
        /// </summary>
        /// <param name="triggerTime">The trap trigger time.</param>
        public void TriggerTrapIfAny(ETrapTriggerTime triggerTime)
        {
            _trapsToTrigger.Clear();

            foreach (TrapSO trap in TrapList)
            {
                if (HasFighter() && trap.TriggerTimes.Contains(triggerTime))
                {
                    _trapsToTrigger.Enqueue(trap);
                }
            }

            TriggerNextTrap();
        }

        private void TriggerNextTrap()
        {

            if (_trapsToTrigger.Count > 0)
            {
                TrapSO trap = _trapsToTrigger.Dequeue();

                trap.Trigger(Fighter, this);

            }
            else
            {
                onTrapTriggered?.Invoke();
            }
        }
        public void HandleAnimationEnd()
        {
            TriggerNextTrap();
        }



        /// <summary>
        /// Update the alterations on the cell.
        /// If a temporary alteration is over, it will be removed.
        /// If a trap triggering on stay is present and a fighter occupies the cell, the trap will be triggered.
        /// </summary>
        public void UpdateAlterations()
        {
            foreach (TrapSO trap in TrapList)
            {
                if (HasFighter() && trap.TriggerTimes.Contains(ETrapTriggerTime.OnStay))
                {
                    trap.Trigger(Fighter, this);
                }
                AlterationsManager.UpdateAlterations();
            }

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
            return Obstacle != null;
        }

        public bool HasTrap()
        {
            return TrapList.Count > 0;
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
