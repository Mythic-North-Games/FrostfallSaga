using FrostfallSaga.Grid;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomHexGrid : AHexGrid
    {
        private KingdomCell _hexKingdomPrefab;
        private KingdomGridGenerator _kingdomGridGenerator;

        public override void GenerateGrid()
        {
            _kingdomGridGenerator = new KingdomGridGenerator(_hexKingdomPrefab, Width, Height, AvailableBiomes,
                transform, NoiseScale, Seed);
            Cells = _kingdomGridGenerator.GeneratorGenerateGrid();
        }

        #region Setup & tear down

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _hexKingdomPrefab ??= Resources.Load<KingdomCell>("Prefabs/Grid/KingdomCell");
            if (!_hexKingdomPrefab) Debug.LogError("KingdomCellPrefab is null.");
        }

        #endregion
    }
}