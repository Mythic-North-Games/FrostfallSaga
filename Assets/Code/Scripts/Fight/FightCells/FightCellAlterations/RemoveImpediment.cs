using System;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that adds an impediment to a cell.
    /// </summary>
    public class RemoveImpediment : AFightCellAlteration
    {

        public override void Apply(FightCell fightCell)
        {
            if (fightCell.TrapList.Count > 0)
            {
                fightCell.TrapList.Clear();
                UnityEngine.Object.Destroy(fightCell.TrapVisibleInstance);
            }
        }

        public override void Remove(FightCell cell)
        {
        }
    }
}