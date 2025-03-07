using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Grid
{
    public class MovePath
    {
        public readonly Cell[] Path;
        private int _currentCellDestinationIndex;

        public MovePath(Cell[] movePath)
        {
            _currentCellDestinationIndex = -1;
            Path = movePath;
        }

        public int PathLength => Path.Length;

        private Cell CurrentDestinationCell => Path[_currentCellDestinationIndex];

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