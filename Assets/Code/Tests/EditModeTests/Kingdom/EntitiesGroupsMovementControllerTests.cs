using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.EditModeTests.Kingdom
{
	public class EntitiesGroupsMovementControllerTests
	{
		private HexGrid grid;

		[SetUp]
		public void Setup()
		{
			grid = CommonTestsHelper.CreatePlainGridForTest(false, 10, 10);
		}

		[TearDown]
		public void TearDown()
		{
			UnityEngine.Object.DestroyImmediate(grid.gameObject);
			grid = null;
		}

		[Test]
		public void GenerateRandomMovePathForEntitiesGroup_NoProhibitedCells_LengthMax_Test()
		{
			// Arrange
			int pathLength = 3;
			HashSet<KingdomCell> prohibitedCells = new();
			EntitiesGroup entitiesGroup = KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)] as KingdomCell, pathLength);

			// Act
			KingdomCell[] path = EntitiesGroupsMovementController.GenerateRandomMovePathForEntitiesGroup(
				grid,
				entitiesGroup,
				prohibitedCells, pathLength
			);

			/// ASSERTS ///
			// Assert path length
			Assert.AreEqual(path.Length, pathLength);

			// Assert all cells are unique
			HashSet<KingdomCell> uniqueCells = new();
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
			Assert.True(uniqueCells.All(cell => cell.IsFree()));
		}

		[Test]
		public void GenerateRandomMovePathForEntitiesGroup_ProhibitedCells_LengthMax_Test()
		{
			// Arrange
			int pathLength = 3;
			EntitiesGroup entitiesGroup = KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)] as KingdomCell, pathLength);
			HashSet<KingdomCell> prohibitedCells = new()
			{
				grid.CellsByCoordinates[new(0, 1)] as KingdomCell
			};

			int repeatExperienceCount = 50;
			for (int currentExperienceIndex = 0; currentExperienceIndex < repeatExperienceCount; currentExperienceIndex++)
			{
				// Act
				KingdomCell[] path = EntitiesGroupsMovementController.GenerateRandomMovePathForEntitiesGroup(
					grid,
					entitiesGroup,
					prohibitedCells, pathLength
				);

				/// ASSERTS ///
				// Assert path length
				Assert.AreEqual(path.Length, pathLength);

				// Assert all cells are unique
				HashSet<KingdomCell> uniqueCells = new();
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
				Assert.True(uniqueCells.All(cell => cell.IsFree()));
			}
		}

		[Test]
		public void GenerateRandomMovePathForEntitiesGroup_MinPathLengthLessThanZero_Test()
		{
			// Arrange
			int minPathLength = -3;
			HashSet<KingdomCell> prohibitedCells = new();
			EntitiesGroup entitiesGroup = KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)] as KingdomCell, minPathLength);

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
			int minPathLength = 2;
			EntitiesGroup[] entitiesGroups = {
				KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(0, 0)] as KingdomCell, minPathLength),
				KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(2, 2)] as KingdomCell, minPathLength),
				KingdomTestsHelper.CreateEntitiesGroup(grid.CellsByCoordinates[new(2, 0)] as KingdomCell, minPathLength),
			};

			// Act
			Dictionary<EntitiesGroup, KingdomCell[]> pathPerEntitiesGroup =
				EntitiesGroupsMovementController.GenerateRandomMovePathPerEntitiesGroup(grid, entitiesGroups, minPathLength);

			/// ASSERTS ///
			for (int testCount = 0; testCount < 20; testCount++)
			{
				AssertPathForMultipleEntitiesGroup(grid, pathPerEntitiesGroup);
			}

		}

		private void AssertPathForMultipleEntitiesGroup(HexGrid grid, Dictionary<EntitiesGroup, KingdomCell[]> pathPerEntitiesGroup)
		{
			foreach (KeyValuePair<EntitiesGroup, KingdomCell[]> item in pathPerEntitiesGroup)
			{
				EntitiesGroup currentEntitiesGroup = item.Key;
				KingdomCell[] path = item.Value;

				// Assert all cells are unique
				HashSet<KingdomCell> uniqueCells = new();
				uniqueCells.UnionWith(path);
				Assert.AreEqual(uniqueCells.Count, path.Length);

				// Assert path does not contains prohibited cells
				foreach (
					KeyValuePair<EntitiesGroup, KingdomCell[]> otherPathPerEntitiesGroup in pathPerEntitiesGroup.Where(
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
				Assert.True(uniqueCells.All(cell => cell.IsFree()));
			}
		}
	}
}