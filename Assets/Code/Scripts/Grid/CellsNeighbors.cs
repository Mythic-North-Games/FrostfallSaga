using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Expose methods for getting neighbors of a Cell in an HexGrid.
    /// </summary>
    public static class CellsNeighbors
    {
        public static Vector2Int[] directionsToCheckIfHeightOdd = new Vector2Int[]
        {
            new(1, 0), new(-1, 0),
            new(1, 1), new(0, -1),
            new(1, -1), new(0, 1)
        };

        public static Vector2Int[] directionsToCheckIfHeightEven = new Vector2Int[]
        {
            new(1, 0), new(-1, 0),
            new(0, 1), new(-1, -1),
            new(0, -1), new(-1, 1)
        };

        /// <summary>
        /// Compute and returns the current cell neighbors in the given grid.
        /// </summary>
        /// <param name="hexGrid">The grid the current cell is considered inside.</param>
        /// <param name="cellToGetTheNeighbors">The cell to get the neigbors from.</param>
        /// <param name="includeInaccessibleNeighbors">If the inaccessible cells should be included.</param>
        /// <param name="includeHeightInaccessibleNeighbors">If only the height inaccessible cells should be included.</param>
        /// <param name="includeOccupiedNeighbors">If the occupied cells should be included.</param>
        /// <returns>The current cell neighbors in the given grid.</returns>
        public static Cell[] GetNeighbors(
            HexGrid hexGrid,
            Cell cellToGetTheNeighbors,
            bool includeInaccessibleNeighbors = false,
            bool includeHeightInaccessibleNeighbors = false,
            bool includeOccupiedNeighbors = false,
            Cell[] mandatoryCells = null
        )
        {
            mandatoryCells ??= new Cell[0];

            List<Cell> neighbors = new();
            Vector2Int[] directionsToCheck = cellToGetTheNeighbors.Coordinates.y % 2 == 0 ?
                directionsToCheckIfHeightEven :
                directionsToCheckIfHeightOdd;

            foreach (Vector2Int direction in directionsToCheck)
            {
                Vector2Int neighborCoord = cellToGetTheNeighbors.Coordinates + direction;
                Dictionary<Vector2Int, Cell> cellsByCoordinates = hexGrid.CellsByCoordinates;
                if (cellsByCoordinates.ContainsKey(neighborCoord))
                {
                    Cell currentNeighbor = cellsByCoordinates[neighborCoord];
                    if (
                        mandatoryCells.Contains(currentNeighbor) ||
                        (includeOccupiedNeighbors || currentNeighbor.IsFree()) &&
                        (includeInaccessibleNeighbors || currentNeighbor.IsTerrainAccessible()) &&
                        (
                            includeHeightInaccessibleNeighbors ||
                            GetHeightDifference(cellToGetTheNeighbors, currentNeighbor) <= 1
                        )
                    )
                    {
                        neighbors.Add(currentNeighbor);
                    }
                }
            }

            return neighbors.ToArray();
        }

        public static List<Cell> GetCellsInRange(int range, Cell cell)
        {
            HashSet<Cell> visitedCells = new HashSet<Cell>();
            Queue<Cell> queue = new Queue<Cell>();

            queue.Enqueue(cell);
            visitedCells.Add(cell);
            for (int i = 0; i < range; i++)
            {
                int levelSize = queue.Count;
                for (int j = 0; j < levelSize; j++)
                {
                    Cell currentCell = queue.Dequeue();
                    Cell[] neighbors = GetNeighbors(currentCell.ParentGrid, currentCell);

                    foreach (Cell neighbor in neighbors)
                    {
                        if (!visitedCells.Contains(neighbor))
                        {
                            visitedCells.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            visitedCells.Remove(cell);
            return visitedCells.ToList();
        }

        /// <summary>
        /// Compute and returns the absolute height difference between two cells.
        /// </summary>
        /// <param name="cell1">One of the cell.</param>
        /// <param name="cell2">The other cell.</param>
        /// <returns>The absolute height difference between the given two cells.</returns>
        public static int GetHeightDifference(Cell cell1, Cell cell2)
        {
            return Math.Abs((int)cell2.Height - (int)cell1.Height);
        }
    }
}