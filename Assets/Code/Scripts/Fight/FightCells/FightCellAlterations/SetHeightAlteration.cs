using System;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that sets the height of a cell to a specific value.
    /// </summary>
    public class SetHeightAlteration : AFightCellAlteration
    {
        [field: SerializeField]
        [field: Header("Height alteration definition")]
        public ECellHeight TargetHeight { get; private set; }

        private ECellHeight _previousHeight;

        public SetHeightAlteration() : base(
            "Set height",
            "Sets the height of the cell to a specific value.",
            null,
            false,
            0,
            true,
            true
        )
        {
        }

        public SetHeightAlteration(
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
            cell.UpdateHeight(TargetHeight, Duration);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight, Duration);
            onAlterationRemoved?.Invoke(cell, this);
        }
    }
}