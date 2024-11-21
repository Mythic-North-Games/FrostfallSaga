using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.EntitiesGroups;
using NUnit.Framework;

namespace FrostfallSaga.EditModeTests.Kingdom
{
    public class EntitiesGroupsMovementControllerTests
    {
        [Test]
        public void GenerateRandomMovePathForEntitiesGroup_NoProhibitedCells_LengthMax_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(10, 10);
            int pathLength = 3;
            HashSet<Cell> prohibitedCells = new();
            EntitiesGroup entitiesGroup = KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)], pathLength);

            // Act
            Cell[] path = EntitiesGroupsMovementController.GenerateRandomMovePathForEntitiesGroup(
                grid,
                entitiesGroup,
                prohibitedCells, pathLength
            );

            /// ASSERTS ///
            // Assert path length
            Assert.AreEqual(path.Length, pathLength);

            // Assert all cells are unique
            HashSet<Cell> uniqueCells = new();
            uniqueCells.UnionWith(path);
            Assert.AreEqual(uniqueCells.Count, path.Length);

            // Assert path does not contains prohibited cells
            Assert.False(path.All(cellOfPath => prohibitedCells.Contains(cellOfPath)));

            // Assert all cells form a path (neighbors)
            for (int i = 0; i < pathLength - 1; i++)
            {
                Assert.True(CellsNeighbors.GetNeighbors(grid, path[i]).Contains(path[i + 1]));
            }

            // Assert all cells are accessibles
            Assert.True(uniqueCells.All(cell => cell.IsAccessible));
        }

        [Test]
        public void GenerateRandomMovePathForEntitiesGroup_ProhibitedCells_LengthMax_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(10, 10);
            int pathLength = 3;
            EntitiesGroup entitiesGroup = KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)], pathLength);
            HashSet<Cell> prohibitedCells = new()
            {
                grid.CellsByCoordinates[new(0, 1)]
            };

            int repeatExperienceCount = 50;
            for (int currentExperienceIndex = 0; currentExperienceIndex < repeatExperienceCount; currentExperienceIndex++)
            {
                // Act
                Cell[] path = EntitiesGroupsMovementController.GenerateRandomMovePathForEntitiesGroup(
                    grid,
                    entitiesGroup,
                    prohibitedCells, pathLength
                );

                /// ASSERTS ///
                // Assert path length
                Assert.AreEqual(path.Length, pathLength);

                // Assert all cells are unique
                HashSet<Cell> uniqueCells = new();
                uniqueCells.UnionWith(path);
                Assert.AreEqual(uniqueCells.Count, path.Length);

                // Assert path does not contains prohibited cells
                Assert.False(path.All(cellOfPath => prohibitedCells.Contains(cellOfPath)));

                // Assert all cells form a path (neighbors)
                for (int i = 0; i < pathLength - 1; i++)
                {
                    Assert.True(CellsNeighbors.GetNeighbors(grid, path[i]).Contains(path[i + 1]));
                }

                // Assert all cells are accessibles
                Assert.True(uniqueCells.All(cell => cell.IsAccessible));
            }
        }

        [Test]
        public void GenerateRandomMovePathForEntitiesGroup_MinPathLengthLessThanZero_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(10, 10);
            int minPathLength = -3;
            HashSet<Cell> prohibitedCells = new();
            EntitiesGroup entitiesGroup = KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)], minPathLength);

            // Act
            Assert.Throws<ArgumentException>(() => EntitiesGroupsMovementController.GenerateRandomMovePathForEntitiesGroup(
                grid,
                entitiesGroup,
                prohibitedCells, minPathLength
            ));
        }

        [Test]
        public void GenerateRandomMovePathPerEntitiesGroup_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(10, 10);
            int minPathLength = 2;
            EntitiesGroup[] entitiesGroups = {
                KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)], minPathLength),
                KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(2, 2)], minPathLength),
                KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(2, 0)], minPathLength),
            };

            // Act
            Dictionary<EntitiesGroup, Cell[]> pathPerEntitiesGroup = EntitiesGroupsMovementController.GenerateRandomMovePathPerEntitiesGroup(grid, entitiesGroups, minPathLength);

            /// ASSERTS ///
            for (int testCount = 0; testCount < 20; testCount++)
            {
                AssertPathForMultipleEntitiesGroup(grid, pathPerEntitiesGroup);
            }

        }

        private void AssertPathForMultipleEntitiesGroup(HexGrid grid, Dictionary<EntitiesGroup, Cell[]> pathPerEntitiesGroup)
        {
            foreach (KeyValuePair<EntitiesGroup, Cell[]> item in pathPerEntitiesGroup)
            {
                EntitiesGroup currentEntitiesGroup = item.Key;
                Cell[] path = item.Value;

                // Assert all cells are unique
                HashSet<Cell> uniqueCells = new();
                uniqueCells.UnionWith(path);
                Assert.AreEqual(uniqueCells.Count, path.Length);

                // Assert path does not contains prohibited cells
                foreach (
                    KeyValuePair<EntitiesGroup, Cell[]> otherPathPerEntitiesGroup in pathPerEntitiesGroup.Where(
                        item => item.Key != currentEntitiesGroup
                    )
                )
                {
                    Assert.False(path.All(cellOfPath => otherPathPerEntitiesGroup.Value.Contains(cellOfPath)));
                }

                // Assert all cells form a path (neighbors)
                for (int i = 0; i < path.Length - 1; i++)
                {
                    Assert.True(CellsNeighbors.GetNeighbors(grid, path[i]).Contains(path[i + 1]));
                }

                // Assert all cells are accessibles
                Assert.True(uniqueCells.All(cell => cell.IsAccessible));
            }
        }
    }
}