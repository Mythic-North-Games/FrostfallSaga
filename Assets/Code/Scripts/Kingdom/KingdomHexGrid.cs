using FrostfallSaga.Grid;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomHexGrid : AHexGrid
    {
        [field: SerializeField] public GameObject[] InterestPoints { get; private set; }
        private KingdomCell _hexKingdomPrefab;
        private KingdomGridGenerator kingdomGridGenerator;

        public override void GenerateGrid()
        {
            kingdomGridGenerator = new(_hexKingdomPrefab, Width, Height, AvailableBiomes, this.transform, NoiseScale, Seed, InterestPoints);
            Cells = kingdomGridGenerator.GeneratorGenerateGrid();
        }

        public void GenerateInterestPoints()
        {
            if (kingdomGridGenerator == null)
            {
                Debug.LogError("KingdomGrdiGenerator is null. Make sure to use GenerateGrid() before to use this method.");
                return;
            }
            if (InterestPoints.Length == 0 || InterestPoints == null) 
            {
                Debug.LogWarning("Cannot generate Interest Points, cause Interest Points are empty or null.");
                return;
            }
            kingdomGridGenerator.GeneratorGenerateInterestPoints(Cells);
        }

        public void GenerateInterestPoints()
        {
            if (kingdomGridGenerator == null)
            {
                Debug.LogError("KingdomGrdiGenerator is null. Make sure to use GenerateGrid() before to use this method");
                return;
            }
            kingdomGridGenerator.SetupInterestPoints(Cells);
        }

        #region Setup & tear down
        private void Awake()
        {
            Initialize();
        }
        public void Initialize()
        {
            if (InterestPoints == null || InterestPoints.Length == 0)
            {
                Debug.LogWarning("No Interest Points setup.");
            }
            _hexKingdomPrefab ??= Resources.Load<KingdomCell>("Prefabs/Grid/KingdomCell");
            if (_hexKingdomPrefab == null)
            {
                Debug.LogError("KingdomCellPrefab is null.");
            }
        }
        #endregion
    }
}
