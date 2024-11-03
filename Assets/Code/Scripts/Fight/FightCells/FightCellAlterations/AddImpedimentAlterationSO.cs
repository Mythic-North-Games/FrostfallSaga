using UnityEngine;
using FrostfallSaga.Fight.FightCells.Impediments;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [CreateAssetMenu(
        fileName = "AddImpedimentAlteration",
        menuName = "ScriptableObjects/Fight/CellAlterations/AddImpedimentAlteration",
        order = 0
    )]
    /// <summary>
    /// Alteration that adds an impediment to a cell.
    /// </summary>
    public class AddImpedimentAlterationSO : AFightCellAlterationSO
    {
        [field: SerializeField, Header("Impediment alteration definition")]
        public AImpedimentSO Impediment { get; private set; }

        public override void Apply(FightCell cell)
        {
            Impediment.ApplyOnCell(cell);
            onAlterationApplied?.Invoke(cell);
        }

        public override void Remove(FightCell cell)
        {
            Impediment.Destroy(cell);
            onAlterationRemoved?.Invoke(cell);
        }
    }
}