using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [CreateAssetMenu(
        fileName = "InvertHeightAlteration",
        menuName = "ScriptableObjects/Fight/CellAlterations/InvertHeightAlteration",
        order = 0
    )]
    /// <summary>
    /// Alteration that inverts the height of a cell.
    /// </summary>
    public class InvertHeightAlterationSO : AFightCellAlterationSO
    {
        private ECellHeight _previousHeight;

        public override void Apply(FightCell cell)
        {
            _previousHeight = cell.Height;
            cell.UpdateHeight(GetInvertedHeight(cell.Height));
            onAlterationApplied?.Invoke(cell);
        }

        public override void Remove(FightCell cell)
        {
            cell.UpdateHeight(_previousHeight);
            onAlterationRemoved?.Invoke(cell);
        }

        private ECellHeight GetInvertedHeight(ECellHeight currentHeight)
        {
            return currentHeight switch
            {
                ECellHeight.LOW => ECellHeight.HIGH,
                ECellHeight.MEDIUM => ECellHeight.MEDIUM,
                ECellHeight.HIGH => ECellHeight.LOW,
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}