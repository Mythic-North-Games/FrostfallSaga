using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Kingdom
{
	public class MovePath
	{
		public int _currentCellDestinationIndex;
		public readonly Cell[] path;

		public MovePath(Cell[] movePath)
		{
			_currentCellDestinationIndex = -1;
			path = movePath;
		}

		public int PathLength
		{
			get => path.Length;
		}

		public Cell CurrentDestinationCell
		{
			get => path[_currentCellDestinationIndex];
		}

		public bool IsLastMove
		{
			get => _currentCellDestinationIndex == PathLength - 1;
		}

		public Cell GetNextCellInPath()
		{
			if (IsLastMove || PathLength == 0)
			{
				return null;
			}

			_currentCellDestinationIndex += 1;
			return CurrentDestinationCell;
		}
	}
}