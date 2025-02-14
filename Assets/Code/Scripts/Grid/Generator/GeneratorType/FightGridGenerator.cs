using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Procedural;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class FightGridGenerator : BaseGridGenerator
    {
        private PerlinTerrainManager _perlinTerrainManager;
        private int _lastSeed;
        private int _gridWidth;
        private int _gridHeight;
        private BiomeTypeSO[] _availableBiomes;

        public FightGridGenerator(int gridWidth, int gridHeight, BiomeTypeSO[] availableBiomes, float? noiseScale, int? seed)
            : base(noiseScale, seed)
        {
            _perlinTerrainManager = new PerlinTerrainManager(noiseScale.Value, seed.Value);
            _lastSeed = seed.Value;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            _availableBiomes = availableBiomes;
        }

        public override Dictionary<Vector2Int, Cell> GenerateGrid(Cell hexPrefab, float hexSize = 2)
        {
            // LOGIQUE DE CREATION DE GRILLE //
            throw new System.NotImplementedException();
        }
    }
}
