using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public static class CellAnalysis
    {
        private static readonly HexDirection[] Direction =
        {
            HexDirection.WEST, HexDirection.NORTHWEST,
            HexDirection.NORTHEAST, HexDirection.EAST,
            HexDirection.SOUTHEAST, HexDirection.SOUTHWEST
        };

        public static Dictionary<HexDirection, Cell> AnalyzeAtCell(Cell targetCell, AHexGrid grid, bool isPrintAnalysis)
        {
            Dictionary<HexDirection, Cell> cellsByDirections = new();
            Cell[] neighborCells = CellsNeighbors.GetNeighborsInClockwiseOrder(grid, targetCell);
            for (int i = 0; i < neighborCells.Length; i++)
            {
                Cell neighbor = neighborCells[i];
                if (neighbor)
                {
                    cellsByDirections[Direction[i]] = neighbor;
                }
            }

            if (isPrintAnalysis)
                PrintAnalysisDict(cellsByDirections);

            return cellsByDirections;
        }

        public static void PrintAnalysisDict(Dictionary<HexDirection, Cell> cellsByDirections)
        {
            foreach (KeyValuePair<HexDirection, Cell> item in cellsByDirections)
            {
                string cellName = item.Value ? item.Value.name : "NULL";
                string terrainTypeName = item.Value ? item.Value.TerrainType.name : "NULL";

                Debug.Log(
                    $"HexDirection : {item.Key} || Cell name : {cellName} || Cell Terrain name : {terrainTypeName}");
            }
        }
    }
}