using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class DungeonGridGenerator : BaseGridGenerator
    {
        private int _gridWidth;
        private int _gridHeight;
        private BiomeTypeSO[] _availableBiomes;
        public DungeonGridGenerator(int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes) : base(gridWidth, gridHeight)
        {
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _availableBiomes = availableBiomes;
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid(Cell hexPrefab, float hexSize = 2.0f)
        {
            // LOGIQUE DE CREATION DE GRILLE //
            throw new System.NotImplementedException();
        }
    }
}
