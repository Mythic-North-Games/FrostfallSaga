using System.Collections.Generic;
using System.Linq;
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
            ClearCells();
            _kingdomGridGenerator = new KingdomGridGenerator(_hexKingdomPrefab, Width, Height, AvailableBiomes,
                transform, NoiseScale, Seed);
            Cells = _kingdomGridGenerator.GenerateGrid();
        }

        /// <summary>
        ///     Retrieve all free cells from KingdomGrid
        /// </summary>
        public List<KingdomCell> GetFreeCells()
        {
            return Cells.Values.OfType<KingdomCell>().Where(cell => cell.IsFree() && cell.IsTerrainAccessible()).ToList();
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