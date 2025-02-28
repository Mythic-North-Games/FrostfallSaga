using FrostfallSaga.Grid;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomHexGrid : AHexGrid
    {
        [field: SerializeField] public GameObject[] InterestPoints { get; private set; }
        private KingdomCell _hexKingdomPrefab;

        public override void GenerateGrid()
        {
            Debug.Log("Generate Kingdom Grid...");
            KingdomGridGenerator kingdomGridGenerator = new(_hexKingdomPrefab, Width, Height, AvailableBiomes, this.transform, NoiseScale, Seed, InterestPoints);
            Cells = kingdomGridGenerator.GenerateGrid();
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
                Debug.LogWarning("No Interest Points setup");
            }
            _hexKingdomPrefab = Resources.Load<KingdomCell>("Prefabs/Grid/KingdomCell");
        }
        #endregion
    }
}
