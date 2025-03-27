using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.Fight
{
    public class FightHexGrid : AHexGrid
    {
        private FightGridGenerator _fightGridGenerator;
        private FightCell _hexFightPrefab;


        public override void GenerateGrid()
        {
            Debug.Log("Generate Grid...");
            _fightGridGenerator = new FightGridGenerator(_hexFightPrefab, Width, Height, AvailableBiomes, transform, NoiseScale, Seed);
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
        }

        #endregion
    }
}