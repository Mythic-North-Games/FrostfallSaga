using FrostfallSaga.Grid.Cells;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public interface IGridGenerator
    {
        Dictionary<Vector2Int, Cell> GenerateGrid(Cell hexPrefab, float hexSize = 2.0f);
    }

}
