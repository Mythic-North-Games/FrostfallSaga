using UnityEngine;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    /// <summary>
    /// Alteration that sets the terrain of a cell.
    /// </summary>
    public class SetTerrainAlteration : AFightCellAlteration
    {
        [field: SerializeField, Header("Set terrain alteration definition")]
        public TerrainTypeSO TerrainType { get; private set; }

        private TerrainTypeSO _previousTerrainType;

        public SetTerrainAlteration()
        {
            CanBeReplaced = true;
            CanApplyWithFighter = true;
        }

        public SetTerrainAlteration(
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
            _previousTerrainType = cell.TerrainType;
            cell.SetTerrain(TerrainType);
            onAlterationApplied?.Invoke(cell, this);
        }

        public override void Remove(FightCell cell)
        {
            cell.SetTerrain(_previousTerrainType);
            onAlterationRemoved?.Invoke(cell, this);
        }
    }
}