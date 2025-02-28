using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class DungeonGridGenerator : ABaseGridGenerator
    {
        public DungeonGridGenerator(Cell hexPrefab, int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, Transform parentGrid) 
            : base(hexPrefab, gridWidth, gridHeight, availableBiomes, parentGrid) {}

        public override Dictionary<Vector2Int, Cell> GeneratorGenerateGrid()
        {
            // LOGIQUE DE CREATION DE GRILLE //
            throw new System.NotImplementedException();
        }
    }
}
