using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    ///     Expose methods for getting neighbors of a Cell in an HexGrid.
    /// </summary>
    public static class CellsNeighbors
    {
        private static readonly Vector2Int[] DirectionsToCheckIfHeightOdd =
        {
            new(1, 0),
            new(-1, 0),
            new(1, 1),
            new(0, -1),
            new(1, -1),
            new(0, 1)
        };

        private static readonly Vector2Int[] DirectionsToCheckIfHeightEven =
        {
            new(1, 0),
            new(-1, 0),
            new(0, 1),
            new(-1, -1),
            new(0, -1),
            new(-1, 1)
        };

        private static readonly Vector2Int[] DirectionsToClock =
        {
            new(-1, 0), // WEST
            new(-1, 1), // NORTHWEST
            new(0, 1), // NORTHEAST
            new(1, 0), // EAST
            new(1, -1), // SOUTHEAST
            new(0, -1) // SOUTHWEST
        };

        /// <summary>
        ///     Compute and returns the current cell neighbors in the given grid.
        /// </summary>
        /// <param name="hexGrid">The grid the current cell is considered inside.</param>
        /// <param name="cellToGetTheNeighbors">The cell to get the neigbors from.</param>
        /// <param name="includeInaccessibleNeighbors">If the inaccessible cells should be included.</param>
        /// <param name="includeHeightInaccessibleNeighbors">If only the height inaccessible cells should be included.</param>
        /// <param name="includeOccupiedNeighbors">If the occupied cells should be included.</param>
        /// <param name="mandatoryCells">Cells are mandatory</param>
        /// <returns>The current cell neighbors in the given grid.</returns>
        public static Cell[] GetNeighbors(
            AHexGrid hexGrid,
            Cell cellToGetTheNeighbors,
            bool includeInaccessibleNeighbors = false,
            bool includeHeightInaccessibleNeighbors = false,
            bool includeOccupiedNeighbors = false,
            Cell[] mandatoryCells = null
        )
        {
            mandatoryCells ??= Array.Empty<Cell>();

            List<Cell> neighbors = new();
            Vector2Int[] directionsToCheck = cellToGetTheNeighbors.Coordinates.y % 2 == 0
                ? DirectionsToCheckIfHeightEven
                : DirectionsToCheckIfHeightOdd;

            foreach (Vector2Int direction in directionsToCheck)
            {
                Vector2Int neighborCoord = cellToGetTheNeighbors.Coordinates + direction;
                Dictionary<Vector2Int, Cell> cellsByCoordinates = hexGrid.Cells;
                if (cellsByCoordinates.TryGetValue(neighborCoord, out Cell currentNeighbor))
                    if (
                        mandatoryCells.Contains(currentNeighbor) ||
                        ((includeOccupiedNeighbors || currentNeighbor.IsFree()) &&
                         (includeInaccessibleNeighbors || currentNeighbor.IsTerrainAccessible()) &&
                         (
                             includeHeightInaccessibleNeighbors ||
                             GetHeightDifference(cellToGetTheNeighbors, currentNeighbor) <= 1
                         ))
                    )
                        neighbors.Add(currentNeighbor);
            }

            return neighbors.ToArray();
        }

        /// <summary>
        ///     Compute and returns the current cell neighbors in a fixed array of 6 elements, ordered as:
        ///     Left -> Top Left -> Top Right -> Right -> Bottom Right -> Bottom Left.
        ///     If a neighbor does not exist, the corresponding index will be null.
        /// </summary>
        /// <param name="hexGrid">The grid the current cell is considered inside.</param>
        /// <param name="cell">The cell to get the neighbors from.</param>
        /// <returns>A fixed array of 6 neighbors, with null for missing neighbors.</returns>
        public static Cell[] GetNeighborsInClockwiseOrder(AHexGrid hexGrid, Cell cell)
        {
            Cell[] orderedNeighbors = new Cell[6];
            Dictionary<Vector2Int, Cell> cellsByCoordinates = hexGrid.Cells;

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighborCoord = cell.Coordinates + DirectionsToClock[i];
                orderedNeighbors[i] = cellsByCoordinates.GetValueOrDefault(neighborCoord);
            }

            return orderedNeighbors;
        }

        /// <summary>
        ///     Compute and returns the absolute height difference between two cells.
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