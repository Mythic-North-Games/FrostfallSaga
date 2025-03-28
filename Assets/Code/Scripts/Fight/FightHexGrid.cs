using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.Fight
{
    public class FightHexGrid : AHexGrid
    {
        [SerializeField] private TerrainTypeSO defaultTerrainType;
        private FightGridGenerator _fightGridGenerator;
        private FightCell _hexFightPrefab;

        public override void GenerateGrid()
        {
            Debug.Log("Generate Grid...");
            _fightGridGenerator = new FightGridGenerator(_hexFightPrefab, Width, Height, AvailableBiomes, transform, NoiseScale, Seed, defaultTerrainType);
            Cells = _fightGridGenerator.GenerateGrid();
        }

        #region Setup & tear down

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _hexFightPrefab = Resources.Load<FightCell>("Prefabs/Grid/FightCell");
            defaultTerrainType ??= Resources.Load<TerrainTypeSO>("ScriptableObjects/Grid/Terrain/TerrainTypePlain");
        }

        #endregion
    }
}