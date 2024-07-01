using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Expose methods for pathfinding between Cell in an HexGrid.
    /// </summary>
    public static class CellsPathFinding
    {
        /// <summary>
        /// Returns the shorter path between the given two cells that are inside the given grid.
        /// </summary>
        /// <param name="hexGrid">The grid that contains at least the two cells.</param>
        /// <param name="startCell">One of the two cell to find the shorter path between.</param>
        /// <param name="endCell">The other cell to find the shorter path between.</param>
        /// <returns>An ordered array of Cell representing the shorter path from the startCell to the endCell.</returns>
        public static Cell[] GetShorterPath(HexGrid hexGrid, Cell startCell, Cell endCell)
        {
            PriorityQueue<Cell> frontier = new();
            frontier.Enqueue(startCell, 0);

            Dictionary<Cell, Cell> cameFrom = new();
            Dictionary<Cell, float> costSoFar = new();

            cameFrom[startCell] = null;
            costSoFar[startCell] = 0;

            while (frontier.Count > 0)
            {
                Cell currentCell = frontier.Dequeue();

                if (currentCell == endCell)
                {
                    break;
                }

                foreach (Cell neighbor in CellsNeighbors.GetNeighbors(hexGrid, currentCell))
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
                return new Cell[0];
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