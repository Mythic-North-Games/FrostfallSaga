using FrostfallSaga.Grid;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    /// <summary>
    ///     Alteration that sets the terrain of a cell.
    /// </summary>
    public class SetTerrainAlteration : AFightCellAlteration
    {
        private TerrainTypeSO _previousTerrainType;

        public SetTerrainAlteration() : base(
            "Set terrain",
            "Sets the terrain of the cell.",
            null,
            false,
            0,
            true,
            true
        )
        {
        }

        public SetTerrainAlteration(
            string name,
            string description,
            Sprite icon,
            bool isPermanent,
            int duration
        ) : base(name, description, icon, isPermanent, duration, true, true)
        {
        }

        [field: SerializeField]
        [field: Header("Set terrain alteration definition")]
        public TerrainTypeSO TerrainType { get; private set; }

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