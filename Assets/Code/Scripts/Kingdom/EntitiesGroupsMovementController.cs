using System;
using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
	public class EntitiesGroupsMovementController
	{
		public Action OnAllEntitiesMoved;
		public Action<EnemiesGroup, bool> OnEnemiesGroupEncountered;

		private HexGrid _kingdomGrid;
		private EntitiesGroup _heroGroup;
		private MovePath _currentHeroGroupMovePath;
		private EnemiesGroup[] _enemiesGroupsToMove;
		private readonly Dictionary<EnemiesGroup, MovePath> _currentPathPerEnemiesGroup = new();

		private static readonly Random randomizer = new();

		public void MakeHeroGroupThenEnemiesGroupMove(
			HexGrid kingdomGrid,
			EntitiesGroup heroGroup,
			MovePath heroGroupMovePath,
			EnemiesGroup[] enemiesGroupsToMove
		)
        {
			_kingdomGrid = kingdomGrid;
			_heroGroup = heroGroup;
			_currentHeroGroupMovePath = heroGroupMovePath;
			_enemiesGroupsToMove = enemiesGroupsToMove;

			BindEntitiesGroupsMovementEvents();
			MakeHeroGroupMove();
        }

		private void MakeHeroGroupMove()
		{
			Cell cellToMoveTo = _currentHeroGroupMovePath.GetNextCellInPath();

            // Check if collide with enemies group
            EnemiesGroup collidingEnemiesGroup = GetEnemiesGroupThatWillCollide(cellToMoveTo);
            if (collidingEnemiesGroup != null)
            {
				UnbindEntitiesGroupsMovementEvents();
                OnEnemiesGroupEncountered?.Invoke(collidingEnemiesGroup, true);
            }
            else
            {
                _heroGroup.MoveToCell(cellToMoveTo, _currentHeroGroupMovePath.IsLastMove);
            }
		}

        private void OnHeroGroupMoved(EntitiesGroup _heroGroup, Cell _destinationCell)
        {
            if (!_currentHeroGroupMovePath.IsLastMove)
            {
                MakeHeroGroupMove();
            }
            else
            {
                MakeAllEnemiesGroupsMoveSimultaneously();
            }
        }

        private void MakeAllEnemiesGroupsMoveSimultaneously()
        {
            Dictionary<EntitiesGroup, Cell[]> movePathPerEnemiesGroup = GenerateRandomMovePathPerEntitiesGroup(
				_kingdomGrid,
				_enemiesGroupsToMove
			);
            foreach (KeyValuePair<EntitiesGroup, Cell[]> item in movePathPerEnemiesGroup)
            {
                _currentPathPerEnemiesGroup.Add((EnemiesGroup)item.Key, new(item.Value));
            }

            bool atLeastOneEnemiesGroupMoved = false;
            foreach (KeyValuePair<EnemiesGroup, MovePath> item in _currentPathPerEnemiesGroup)
            {
                if (item.Value.PathLength > 0)  // Sometimes enemies groups don't move :)
                {
                    MakeEnemiesGroupMove(item.Key);
                    atLeastOneEnemiesGroupMoved = true;
                }
            }

            if (!atLeastOneEnemiesGroupMoved)
            {
                EndMovementProcess();
            }
        }

        private void MakeEnemiesGroupMove(EnemiesGroup enemiesGroup)
        {
            MovePath enemiesGroupMovePath = _currentPathPerEnemiesGroup[enemiesGroup];
            Cell cellToMoveTo = enemiesGroupMovePath.GetNextCellInPath();
            if (cellToMoveTo == _heroGroup.Cell)
            {
				UnbindEntitiesGroupsMovementEvents();
                OnEnemiesGroupEncountered?.Invoke(enemiesGroup, false);
            }
            else
            {
                enemiesGroup.MoveToCell(cellToMoveTo, enemiesGroupMovePath.IsLastMove);
            }
        }

        private void OnEnemiesGroupMoved(EntitiesGroup enemiesGroup, Cell _destinationCell)
        {
            MovePath enemiesGroupMovePath = _currentPathPerEnemiesGroup[(EnemiesGroup)enemiesGroup];
            if (!enemiesGroupMovePath.IsLastMove)
            {
                MakeEnemiesGroupMove((EnemiesGroup)enemiesGroup);
            }
            else
            {
                EndMovementProcess();
            }
        }

		private EnemiesGroup GetEnemiesGroupThatWillCollide(Cell targetCell)
        {
            foreach (EnemiesGroup enemiesGroup in _enemiesGroupsToMove)
            {
                if (enemiesGroup.Cell == targetCell)
                {
                    return enemiesGroup;
                }
            }
            return null;
        }

		private void EndMovementProcess()
		{
			_currentPathPerEnemiesGroup.Clear();
			UnbindEntitiesGroupsMovementEvents();
			OnAllEntitiesMoved?.Invoke();
		}

		#region Entities movements event binding and unbinding
        private void BindEntitiesGroupsMovementEvents()
        {
            _heroGroup.OnEntityGroupMoved += OnHeroGroupMoved;
            foreach (EntitiesGroup entitiesGroup in _enemiesGroupsToMove)
            {
                entitiesGroup.OnEntityGroupMoved += OnEnemiesGroupMoved;
            }
        }

        private void UnbindEntitiesGroupsMovementEvents()
        {
            _heroGroup.OnEntityGroupMoved -= OnHeroGroupMoved;
            foreach (EntitiesGroup entitiesGroup in _enemiesGroupsToMove)
            {
                entitiesGroup.OnEntityGroupMoved -= OnEnemiesGroupMoved;
            }
        }
        #endregion

		/// <summary>
		/// Generate random move paths for the given entities groups inside the given grid.
		/// The generated move paths will not collide with each other.
		/// </summary>
		/// <param name="grid">The grid to move inside.</param>
		/// <param name="entitiesGroups">The entities groups to generate a move path for.</param>
		/// <param name="minPathLength">The minimum path's length for all the entities groups.</param>
		/// <returns>A move path per entities group.</returns>
		public static Dictionary<EntitiesGroup, Cell[]> GenerateRandomMovePathPerEntitiesGroup(
			HexGrid grid,
			EntitiesGroup[] entitiesGroups,
			int minPathLength = 0
		)
		{
			Dictionary<EntitiesGroup, Cell[]> pathPerEntitiesGroup = new();
			HashSet<Cell> cellsCoveredByEntitiesGroups = new();
			foreach (EntitiesGroup entitiesGroup in entitiesGroups)
			{
				cellsCoveredByEntitiesGroups.Add(entitiesGroup.Cell);
			}

			foreach (EntitiesGroup entitiesGroup in entitiesGroups)
			{
				Cell[] path = GenerateRandomMovePathForEntitiesGroup(grid, entitiesGroup, cellsCoveredByEntitiesGroups, minPathLength);
				cellsCoveredByEntitiesGroups.UnionWith(path);
				pathPerEntitiesGroup.Add(entitiesGroup, path);
			}
			return pathPerEntitiesGroup;
		}

		/// <summary>
		/// Computes a random cell's path for the given entities group inside the given grid.
		/// </summary>
		/// <param name="kingdomGrid">The grid to generate the path in.</param>
		/// <param name="entitiesGroup">The entities group to generate a path for.</param>
		/// <param name="prohibitedCells">A list of unique cells that the given entities group can't go through.</param>
		/// <param name="minPathLength">The minimum path length. Zero by default. If greater than entities groups move points</param>
		/// <returns>A table of Cell that represents the path.</returns>
		public static Cell[] GenerateRandomMovePathForEntitiesGroup(
			HexGrid kingdomGrid,
			EntitiesGroup entitiesGroup,
			HashSet<Cell> prohibitedCells,
			int minPathLength = 0
		)
		{
			// To ensure the maximum path length is the entities group move points.
			if (minPathLength > entitiesGroup.MovePoints)
			{
				minPathLength = entitiesGroup.MovePoints;
			}
			else if (minPathLength < 0)
			{
				throw new ArgumentException("minPathLength can't be less than zero.");
			}

			List<Cell> randomMovePath = new();
			int numberOfCellsInPath = randomizer.Next(minPathLength, entitiesGroup.MovePoints);

			Cell currentCellOfPath = entitiesGroup.Cell;
			for (int i = 0; i < numberOfCellsInPath; i++)
			{
				List<Cell> currentCellOfPathNeighbors = new(CellsNeighbors.GetNeighbors(kingdomGrid, currentCellOfPath));
				currentCellOfPathNeighbors.Remove(entitiesGroup.Cell);
				currentCellOfPathNeighbors.RemoveAll(cell => randomMovePath.Contains(cell));
				currentCellOfPathNeighbors.RemoveAll(cell => prohibitedCells.Contains(cell));

				if (currentCellOfPathNeighbors.Count == 0)	// Stop generating path if no cell is available to move.
				{
					break;
				}
				
				Cell neighborCellToAdd = currentCellOfPathNeighbors[randomizer.Next(0, currentCellOfPathNeighbors.Count)];
				randomMovePath.Add(neighborCellToAdd);
				currentCellOfPath = neighborCellToAdd;
			}

			return randomMovePath.ToArray();
		}
	}
}