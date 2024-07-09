using System.Linq;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight
{
    public static class FightCellNeighbors
    {
        /// <summary>
        /// Compute and returns the current cell neighbors in the given grid.
        /// </summary>
        /// <param name="hexGrid">The grid the current cell is considered inside.</param>
        /// <param name="cellToGetTheNeighbors">The cell to get the neigbors from.</param>
        /// <param name="includeInaccessibleNeighbors">If the inaccessible cells should be included.</param>
        /// <param name="includeHeightInaccessibleNeighbors">If only the height inaccessible cells should be included.</param>
        /// <returns>The current cell neighbors in the given grid.</returns>
        public static Cell[] GetNeighbors(
            HexGrid hexGrid,
            Cell cellToGetTheNeighbors,
            bool includeInaccessibleNeighbors = false,
            bool includeHeightInaccessibleNeighbors = false
        )
        {
            Cell[] initialNeighbors = CellsNeighbors.GetNeighbors(
                hexGrid,
                cellToGetTheNeighbors,
                includeInaccessibleNeighbors,
                includeHeightInaccessibleNeighbors
            );
            return initialNeighbors.Where(neighborCell => neighborCell.GetComponent<CellFightBehaviour>().Fighter == null).ToArray();
        }
    }
}