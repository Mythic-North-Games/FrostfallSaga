using System;
using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    ///     Expose methods for pathfinding between Cell in an HexGrid.
    /// </summary>
    public static class CellsPathFinding
    {
        /// <summary>
        ///     Returns the shorter path between the given two cells that are inside the given grid.
        /// </summary>
        /// <param name="hexGrid">The grid that contains at least the two cells.</param>
        /// <param name="startCell">One of the two cell to find the shorter path between.</param>
        /// <param name="endCell">The other cell to find the shorter path between.</param>
        /// <param name="includeInaccessibleNeighbors">If the inaccessible cells should be included.</param>
        /// <param name="includeHeightInaccessibleNeighbors">If only the height inaccessible cells should be included.</param>
        /// <param name="includeOccupiedNeighbors">If the occupied cells should be included.</param>
        /// <param name="checkLastCell">If the endCell should be included even if not free.</param>
        /// <returns>An ordered array of Cell representing the shorter path from the startCell to the endCell.</returns>
        public static Cell[] GetShorterPath(
            AHexGrid hexGrid,
            Cell startCell,
            Cell endCell,
            bool includeInaccessibleNeighbors = false,
            bool includeHeightInaccessibleNeighbors = false,
            bool includeOccupiedNeighbors = false,
            bool checkLastCell = true
        )
        {
            Cell[] mandatoryCells = checkLastCell ? Array.Empty<Cell>() : new[] { endCell };

            PriorityQueue<Cell> frontier = new();
            frontier.Enqueue(startCell, 0);

            Dictionary<Cell, Cell> cameFrom = new();
            Dictionary<Cell, float> costSoFar = new();

            cameFrom[startCell] = null;
            costSoFar[startCell] = 0;

            while (frontier.Count > 0)
            {
                Cell currentCell = frontier.Dequeue();

                if (currentCell == endCell) break;

                foreach (Cell neighbor in CellsNeighbors.GetNeighbors(
                             hexGrid,
                             currentCell,
                             includeInaccessibleNeighbors,
                             includeHeightInaccessibleNeighbors,
                             includeOccupiedNeighbors,
                             mandatoryCells
                         )
                        )
                {
                    float newCost = costSoFar[currentCell] + GetCost(currentCell, neighbor);
                    if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = newCost;
                        float priority = newCost + Heuristic(endCell, neighbor);
                        frontier.Enqueue(neighbor, priority);
                        cameFrom[neighbor] = currentCell;
                    }
                }
            }

            try
            {
                return ReconstructPath(cameFrom, startCell, endCell);
            }
            catch (KeyNotFoundException)
            {
                return Array.Empty<Cell>();
            }
        }

        private static float GetCost(Cell a, Cell b)
        {
            return 1;
        }

        private static float Heuristic(Cell a, Cell b)
        {
            return Vector2Int.Distance(a.Coordinates, b.Coordinates);
        }

        private static Cell[] ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell start, Cell end)
        {
            if (!end.IsTerrainAccessible())
            {
                return Array.Empty<Cell>();
            }
            List<Cell> path = new();
            Cell current = end;
            
            while (current != start)
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Reverse();
            return path.ToArray();
        }
    }
}