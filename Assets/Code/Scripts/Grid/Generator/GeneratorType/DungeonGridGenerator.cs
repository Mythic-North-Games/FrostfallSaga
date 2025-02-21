using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class DungeonGridGenerator : BaseGridGenerator
    {
        public DungeonGridGenerator(int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid) 
            : base(gridWidth, gridHeight, availableBiomes, parentGrid) {}

        public override Dictionary<Vector2Int, Cell> GenerateGrid(Cell hexPrefab, float hexSize = 2.0f)
        {
            // LOGIQUE DE CREATION DE GRILLE //
            throw new System.NotImplementedException();
        }
    }
}
