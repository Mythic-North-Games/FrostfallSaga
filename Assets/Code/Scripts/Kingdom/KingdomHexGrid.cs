using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.GameState.Grid;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
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

        public void RestoreGridCells(KingdomCellData[] kingdomCellsData)
        {
            Cells = kingdomCellsData
                .Select(cellData => KingdomCellBuilder.BuildKingdomCell(cellData, this))
                .ToDictionary(cell => cell.Coordinates, cell => (Cell)cell);
        }

        /// <summary>
        ///     Retrieve all free cells from KingdomGrid
        /// </summary>
        public List<KingdomCell> GetFreeCells()
        {
            return Cells.Values.OfType<KingdomCell>().Where(cell => cell.IsFree() && cell.IsAccessible).ToList();
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