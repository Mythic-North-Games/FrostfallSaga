using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.InterestPoints;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Kingdom
{
    public class EntitiesGroupsMovementController
    {
        private readonly Dictionary<EntitiesGroup, MovePath> _currentPathPerEnemiesGroup = new();
        private readonly EntitiesGroup _heroGroup;
        private readonly KingdomHexGrid _kingdomGrid;

        private MovePath _currentHeroGroupMovePath;
        private EntitiesGroup[] _enemiesGroupsToMove;
        public Action OnAllEntitiesMoved;
        public Action<EntitiesGroup, bool> OnEnemiesGroupEncountered;
        public Action<InterestPoint> OnInterestPointEncountered;

        public EntitiesGroupsMovementController(KingdomHexGrid kingdomGrid, EntitiesGroup heroGroup)
        {
            _kingdomGrid = kingdomGrid;
            _heroGroup = heroGroup;
        }

        public void MakeHeroGroupThenEnemiesGroupMove(
            MovePath heroGroupMovePath,
            EntitiesGroup[] enemiesGroupsToMove
        )
        {
            _currentHeroGroupMovePath = heroGroupMovePath;
            _enemiesGroupsToMove = enemiesGroupsToMove;
            BindEntitiesGroupsMovementEvents();
            MakeHeroGroupMove();
        }

        private void MakeHeroGroupMove()
        {
            KingdomCell cellToMoveTo = _currentHeroGroupMovePath.GetNextCellInPath() as KingdomCell;

            // Check if collide with enemies group
            if (cellToMoveTo && cellToMoveTo.HasOccupier())
            {
                if (cellToMoveTo.Occupier is EntitiesGroup collidingEnemiesGroup)
                    OnEnemiesGroupEncountered?.Invoke(collidingEnemiesGroup, true);
                else if (cellToMoveTo.Occupier is InterestPoint upcomingInterestPoint)
                    OnInterestPointEncountered?.Invoke(upcomingInterestPoint);
                UnbindEntitiesGroupsMovementEvents();
            }
            else
            {
                _heroGroup.MoveToCell(cellToMoveTo, _currentHeroGroupMovePath.IsLastMove);
            }
        }

        private void OnHeroGroupMoved(EntitiesGroup _heroGroup, Cell _destinationCell)
        {
            if (!_currentHeroGroupMovePath.IsLastMove)
                MakeHeroGroupMove();
            else
                MakeAllEnemiesGroupsMoveSimultaneously();
        }

        private void MakeAllEnemiesGroupsMoveSimultaneously()
        {
            Dictionary<EntitiesGroup, KingdomCell[]> movePathPerEnemiesGroup = GenerateRandomMovePathPerEntitiesGroup(
                _kingdomGrid,
                _enemiesGroupsToMove
            );
            foreach (KeyValuePair<EntitiesGroup, KingdomCell[]> item in movePathPerEnemiesGroup)
                _currentPathPerEnemiesGroup.Add(item.Key, new MovePath(item.Value));

            bool atLeastOneEnemiesGroupMoved = false;
            foreach (KeyValuePair<EntitiesGroup, MovePath> item in _currentPathPerEnemiesGroup)
                if (item.Value.PathLength > 0) // Sometimes enemies groups don't move :)
                {
                    MakeEnemiesGroupMove(item.Key);
                    atLeastOneEnemiesGroupMoved = true;
                }

            if (!atLeastOneEnemiesGroupMoved) EndMovementProcess();
        }


        private void MakeEnemiesGroupMove(EntitiesGroup EntitiesGroup)
        {
            MovePath enemiesGroupMovePath = _currentPathPerEnemiesGroup[EntitiesGroup];
            KingdomCell cellToMoveTo = enemiesGroupMovePath.GetNextCellInPath() as KingdomCell;
            if (cellToMoveTo == _heroGroup.cell)
            {
                UnbindEntitiesGroupsMovementEvents();
                OnEnemiesGroupEncountered?.Invoke(EntitiesGroup, false);
            }
            else
            {
                EntitiesGroup.MoveToCell(cellToMoveTo, enemiesGroupMovePath.IsLastMove);
            }
        }

        private void OnEnemiesGroupMoved(EntitiesGroup EntitiesGroup, Cell _destinationCell)
        {
            MovePath enemiesGroupMovePath = _currentPathPerEnemiesGroup[EntitiesGroup];
            if (!enemiesGroupMovePath.IsLastMove)
                MakeEnemiesGroupMove(EntitiesGroup);
            else if (HaveAllEnnemiesGroupMoved()) EndMovementProcess();
        }

        private bool HaveAllEnnemiesGroupMoved()
        {
            if (_currentPathPerEnemiesGroup.Count == 0) return true;

            foreach (KeyValuePair<EntitiesGroup, MovePath> item in _currentPathPerEnemiesGroup)
                if (item.Value.DoesNextCellExists())
                    return false;

            return true;
        }

        private void EndMovementProcess()
        {
            _currentPathPerEnemiesGroup.Clear();
            UnbindEntitiesGroupsMovementEvents();
            OnAllEntitiesMoved?.Invoke();
        }

        /// <summary>
        ///     Generate random move paths for the given entities groups inside the given grid.
        ///     The generated move paths will not collide with each other.
        /// </summary>
        /// <param name="grid">The grid to move inside.</param>
        /// <param name="entitiesGroups">The entities groups to generate a move path for.</param>
        /// <param name="minPathLength">The minimum path's length for all the entities groups.</param>
        /// <returns>A move path per entities group.</returns>
        public static Dictionary<EntitiesGroup, KingdomCell[]> GenerateRandomMovePathPerEntitiesGroup(
            KingdomHexGrid grid,
            EntitiesGroup[] entitiesGroups,
            int minPathLength = 0
        )
        {
            Dictionary<EntitiesGroup, KingdomCell[]> pathPerEntitiesGroup = new();
            HashSet<KingdomCell> cellsCoveredByEntitiesGroups = new();
            foreach (EntitiesGroup entitiesGroup in entitiesGroups)
            {
                cellsCoveredByEntitiesGroups.Add(entitiesGroup.cell);
            }

            foreach (EntitiesGroup entitiesGroup in entitiesGroups)
            {
                KingdomCell[] path = GenerateRandomMovePathForEntitiesGroup(
                    grid,
                    entitiesGroup,
                    cellsCoveredByEntitiesGroups,
                    minPathLength
                );
                cellsCoveredByEntitiesGroups.UnionWith(path);
                pathPerEntitiesGroup.Add(entitiesGroup, path);
            }

            return pathPerEntitiesGroup;
        }

        /// <summary>
        ///     Computes a random cell's path for the given entities group inside the given grid.
        /// </summary>
        /// <param name="kingdomGrid">The grid to generate the path in.</param>
        /// <param name="entitiesGroup">The entities group to generate a path for.</param>
        /// <param name="minPathLength">The minimum path length. Zero by default. If greater than entities groups move points</param>
        /// <returns>A table of Cell that represents the path.</returns>
        public static KingdomCell[] GenerateRandomMovePathForEntitiesGroup(
            KingdomHexGrid kingdomGrid,
            EntitiesGroup entitiesGroup,
            HashSet<KingdomCell> prohibitedCells,
            int minPathLength = 0
        )
        {
            // To ensure the maximum path length is the entities group move points.
            if (minPathLength > entitiesGroup.movePoints)
                minPathLength = entitiesGroup.movePoints;
            else if (minPathLength < 0) throw new ArgumentException("minPathLength can't be less than zero.");

            List<KingdomCell> randomMovePath = new();
            int numberOfCellsInPath = Randomizer.GetRandomIntBetween(minPathLength, entitiesGroup.movePoints);

            Cell currentCellOfPath = entitiesGroup.cell;
            for (int i = 0; i < numberOfCellsInPath; i++)
            {
                Cell[] neighbors = CellsNeighbors.GetNeighbors(kingdomGrid, currentCellOfPath, includeOccupiedNeighbors: true);
                List<KingdomCell> currentCellOfPathNeighbors = new(Array.ConvertAll(neighbors, cell => cell as KingdomCell));
                currentCellOfPathNeighbors.Remove(entitiesGroup.cell);
                currentCellOfPathNeighbors.RemoveAll(cell => randomMovePath.Contains(cell));
                currentCellOfPathNeighbors.RemoveAll(cell => prohibitedCells.Contains(cell));

                if (currentCellOfPathNeighbors.Count == 0) // Stop generating path if no cell is available to move.
                    break;

                KingdomCell neighborCellToAdd =
                    Randomizer.GetRandomElementFromArray(currentCellOfPathNeighbors.ToArray());
                randomMovePath.Add(neighborCellToAdd);
                currentCellOfPath = neighborCellToAdd;
            }

            return randomMovePath.ToArray();
        }

        #region Entities movements event binding and unbinding

        private void BindEntitiesGroupsMovementEvents()
        {
            _heroGroup.OnEntityGroupMoved += OnHeroGroupMoved;
            foreach (EntitiesGroup entitiesGroup in _enemiesGroupsToMove)
                entitiesGroup.OnEntityGroupMoved += OnEnemiesGroupMoved;
        }

        private void UnbindEntitiesGroupsMovementEvents()
        {
            _heroGroup.OnEntityGroupMoved -= OnHeroGroupMoved;
            foreach (EntitiesGroup entitiesGroup in _enemiesGroupsToMove)
                entitiesGroup.OnEntityGroupMoved -= OnEnemiesGroupMoved;
        }

        #endregion
    }
}