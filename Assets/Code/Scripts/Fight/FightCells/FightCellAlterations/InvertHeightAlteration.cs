using System;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that inverts the height of a cell.
    /// </summary>
    public class InvertHeightAlteration : AFightCellAlteration
    {
        private ECellHeight _previousHeight;

        public InvertHeightAlteration() : base(
            "Invert height",
            "Inverts the height of the cell.",
            null,
            false,
            0,
            true,
            true
        )
        {
        }

        public InvertHeightAlteration(
            string name,
            string description,
            Sprite icon,
            bool isPermanent,
            int duration
        ) : base(name, description, icon, isPermanent, duration, true, true)
        {
        }

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(GetInvertedHeight(cell.Height), 0);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight, 0);
            onAlterationRemoved?.Invoke(cell, this);
        }

        private ECellHeight GetInvertedHeight(ECellHeight currentHeight)
        {
            return currentHeight switch
            {
                ECellHeight.LOW => ECellHeight.HIGH,
                ECellHeight.MEDIUM => ECellHeight.MEDIUM,
                ECellHeight.HIGH => ECellHeight.LOW,
                _ => throw new NotImplementedException()
            };
        }
    }
}