using System;
using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that sets the height of a cell to a random value.
    /// </summary>
    public class RandomHeightAlteration : AFightCellAlteration
    {
        private ECellHeight _previousHeight;

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(
                Randomizer.GetRandomElementFromEnum<ECellHeight>(toExclude: new[] { cell.Height })
            );
            onAlterationApplied?.Invoke(cell);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight);
            onAlterationRemoved?.Invoke(cell);
        }
    }
}