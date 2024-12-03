using System;
using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;
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

        public RandomHeightAlteration()
        {
            CanBeReplaced = true;
            CanApplyWithFighter = true;
        }

        public RandomHeightAlteration(
            bool isPermanent,
            int duration,
            string name,
            string description,
            Sprite icon
        ) : base(isPermanent, duration, true, true, name, description, icon)
        {
        }

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(
                Randomizer.GetRandomElementFromEnum<ECellHeight>(toExclude: new[] { cell.Height })
            ,0);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight,0);
            onAlterationRemoved?.Invoke(cell, this);
        }
    }
}