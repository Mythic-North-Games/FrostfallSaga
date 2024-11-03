using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    public class SetTerrainAlterationSO : AFightCellAlterationSO
    {
        public TerrainTypeSO terrainType;

        private TerrainTypeSO _previousTerrainType;

        public override void Apply(FightCell cell)
        {
            cell.SetTerrain(terrainType);
        }

        public override void Remove(FightCell cell)
        {
            cell.SetTerrain(_previousTerrainType);
        }
    }
}