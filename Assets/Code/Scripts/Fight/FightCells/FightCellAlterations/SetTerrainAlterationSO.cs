using UnityEngine;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    /// <summary>
    /// Alteration that sets the terrain of a cell.
    /// </summary>
    public class SetTerrainAlterationSO : AFightCellAlterationSO
    {
        [field: SerializeField, Header("Set terrain alteration definition")]
        public TerrainTypeSO TerrainType { get; private set; }

        private TerrainTypeSO _previousTerrainType;

        public override void Apply(FightCell cell)
        {
            _previousTerrainType = cell.TerrainType;
            cell.SetTerrain(TerrainType);
            onAlterationApplied?.Invoke(cell);
        }

        public override void Remove(FightCell cell)
        {
            cell.SetTerrain(_previousTerrainType);
            onAlterationRemoved?.Invoke(cell);
        }
    }
}