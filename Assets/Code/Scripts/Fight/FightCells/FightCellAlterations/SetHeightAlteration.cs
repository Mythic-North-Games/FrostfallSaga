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

        [field: SerializeField] public float CellAlterationDuration { get; protected set; }

        private ECellHeight _previousHeight;

        public SetHeightAlteration(
            string name,
            string description,
            Sprite icon,
            bool isPermanent,
            int duration
        ) : base(name, description, icon, isPermanent, duration, true, true)
        {
            CellAlterationDuration = duration;
        }

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(TargetHeight, CellAlterationDuration);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight, CellAlterationDuration);
            onAlterationRemoved?.Invoke(cell, this);
        }
    }
}