using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public abstract class BaseGridGenerator : IGridGenerator
    {
        protected float? NoiseScale { get; set; }
        protected int? Seed { get; set; }

        protected BaseGridGenerator(float? noiseScale, int? seed)
        {
            NoiseScale = noiseScale;
            Seed = seed;
        }
        public abstract Dictionary<Vector2Int, Cell> GenerateGrid(Cell hexPrefab, float hexSize = 2.0f);

    }
}
