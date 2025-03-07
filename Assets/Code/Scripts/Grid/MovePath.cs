using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Grid
{
    public class MovePath
    {
        public readonly Cell[] path;
        public int _currentCellDestinationIndex;

        public MovePath(Cell[] movePath)
        {
            _currentCellDestinationIndex = -1;
            path = movePath;
        }

        public int PathLength => path.Length;

        public Cell CurrentDestinationCell => path[_currentCellDestinationIndex];

        public bool IsLastMove => _currentCellDestinationIndex == PathLength - 1;

        public Cell GetNextCellInPath()
        {
            if (IsLastMove || PathLength == 0) return null;

            _currentCellDestinationIndex += 1;
            return CurrentDestinationCell;
        }

        public bool DoesNextCellExists()
        {
            if (IsLastMove || PathLength == 0) return false;

            return true;
        }
    }
}