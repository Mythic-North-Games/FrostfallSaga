using System;
using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that sets the height of a cell to a specific value.
    /// </summary>
    public class SetHeightAlteration : AFightCellAlteration
    {
        [field: SerializeField, Header("Height alteration definition")]
        public ECellHeight TargetHeight { get; private set; }

        private ECellHeight _previousHeight;

        [field: SerializeField] public float cellAlterationDuration { get; protected set; }

        public SetHeightAlteration()
        {
            CanBeReplaced = true;
            CanApplyWithFighter = true;
        }

        public SetHeightAlteration(
            bool isPermanent,
            int duration,
            float cellAlterationDuration,
            string name,
            string description,
            Sprite icon
        ) : base(isPermanent, duration,  true, true, name, description, icon)
        {
        }

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(TargetHeight, cellAlterationDuration);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight, cellAlterationDuration);
            onAlterationRemoved?.Invoke(cell, this);
        }
    }
}