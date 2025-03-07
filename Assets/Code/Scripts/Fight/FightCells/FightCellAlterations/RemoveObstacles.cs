using System;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that adds an impediment to a cell.
    /// </summary>
    public class RemoveObstacles : AFightCellAlteration
    {
        public override void Apply(FightCell fightCell)
        {
            Duration = 0;
            if (fightCell.HasObstacle())
            {
                fightCell.Obstacle = null;
                UnityEngine.Object.Destroy(fightCell.CurrentImpedimentGameObject);
            }
        }

        public override void Remove(FightCell cell)
        {
        }
    }
}