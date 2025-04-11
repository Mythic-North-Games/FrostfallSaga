using System;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that sets the height of a cell to a random value.
    /// </summary>
    public class RandomHeightAlteration : AFightCellAlteration
    {
        private ECellHeight _previousHeight;

        public RandomHeightAlteration() : base(
            "Random height",
            "Sets the height of the cell to a random value.",
            null,
            false,
            0,
            true,
            true
        )
        {
        }

        public RandomHeightAlteration(
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
            cell.UpdateHeight(
                Randomizer.GetRandomElementFromEnum(new[] { cell.Height })
                , 0);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight, 0);
            onAlterationRemoved?.Invoke(cell, this);
        }
    }
}