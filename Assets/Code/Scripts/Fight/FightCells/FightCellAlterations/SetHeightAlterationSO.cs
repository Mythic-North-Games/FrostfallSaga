using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [CreateAssetMenu(
        fileName = "SetHeightAlteration",
        menuName = "ScriptableObjects/Fight/CellAlterations/SetHeightAlteration",
        order = 0
    )]
    /// <summary>
    /// Alteration that sets the height of a cell to a specific value.
    /// </summary>
    public class SetHeightAlterationSO : AFightCellAlterationSO
    {
        [field: SerializeField, Header("Height alteration definition")]
        public ECellHeight TargetHeight { get; private set; }

        private ECellHeight _previousHeight;

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(TargetHeight);
            onAlterationApplied?.Invoke(cell);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight);
            onAlterationRemoved?.Invoke(cell);
        }
    }
}