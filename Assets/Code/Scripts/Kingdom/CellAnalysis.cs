using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public static class CellAnalysis
    {
        public static Dictionary<HexDirection, Cell> CELLS_BY_DIRECTION = new();

        private static readonly HexDirection[] Direction =
        {
            HexDirection.WEST, HexDirection.NORTHWEST,
            HexDirection.NORTHEAST, HexDirection.EAST,
            HexDirection.SOUTHEAST, HexDirection.SOUTHWEST
        };

        public static void AnalyzeAtCell(Cell targetCell, AHexGrid grid)
        {
            CELLS_BY_DIRECTION.Clear();
            Cell[] neighborCells = CellsNeighbors.GetNeighborsInClockwiseOrder(grid, targetCell);
            for (int i = 0; i < neighborCells.Length; i++)
            {
                Cell neighbor = neighborCells[i];
                if (neighbor)
                {
                    CELLS_BY_DIRECTION[Direction[i]] = neighbor;
                }
            }
        }

        public static void PrintAnalysisDict()
        {
            foreach (KeyValuePair<HexDirection, Cell> item in CELLS_BY_DIRECTION)
            {
                string cellName = item.Value ? item.Value.name : "NULL";
                string terrainTypeName = item.Value ? item.Value.TerrainType.name : "NULL";

                Debug.Log(
                    $"HexDirection : {item.Key} || Cell name : {cellName} || Cell Terrain name : {terrainTypeName}");
            }
        }
    }
}