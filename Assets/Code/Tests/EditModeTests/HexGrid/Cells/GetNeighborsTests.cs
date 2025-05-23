using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using NUnit.Framework;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.Grid.Cells
{
    public class GetNeighborsTests
    {
        private AHexGrid _grid;

        [SetUp]
        public void Setup()
        {
            _grid = CommonTestsHelper.CreateEmptyGridForTest();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_grid.gameObject);
            _grid = null;
        }

        [Test]
        public void GetNeighbors_AllAccessible_InMiddle_Even_Test()
        {
            // Arrange
            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 2)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(2, 2)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(3, 3)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 1)],
                _grid.Cells[new Vector2Int(2, 1)]
            };
            Debug.Log("Debug From Test : " + _grid);
            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerBottomLeft_Even_Test()
        {
            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(0, 0)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(1, 0)],
                _grid.Cells[new Vector2Int(0, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerBottomRight_Even_Test()
        {
            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(4, 0)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 1)],
                _grid.Cells[new Vector2Int(3, 1)],
                _grid.Cells[new Vector2Int(3, 0)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopLeft_Even_Test()
        {
            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(0, 4)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(1, 4)],
                _grid.Cells[new Vector2Int(0, 3)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopRight_Even_Test()
        {
            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(4, 4)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 3)],
                _grid.Cells[new Vector2Int(3, 3)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_SomeInaccessible_Even_Test()
        {
            _grid.Cells[new Vector2Int(2, 2)].Setup(new Vector2Int(2, 2), ECellHeight.LOW,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.LOW,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 2)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(3, 3)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_SomeHeightInaccessible_Even_Test()
        {
            _grid.Cells[new Vector2Int(2, 2)].Setup(new Vector2Int(2, 2), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 2)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(3, 3)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_LowHeightDifference_Even_Test()
        {
            _grid.Cells[new Vector2Int(2, 2)].Setup(new Vector2Int(2, 2), ECellHeight.MEDIUM,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.MEDIUM,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 2)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(2, 2)],
                _grid.Cells[new Vector2Int(3, 1)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(3, 3)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_Isolated_Even_Test()
        {
            _grid.Cells[new Vector2Int(2, 2)].Setup(new Vector2Int(2, 2), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 3)].Setup(new Vector2Int(3, 3), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(4, 2)].Setup(new Vector2Int(4, 2), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 1)].Setup(new Vector2Int(2, 1), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 2)];
            Cell[] expectedNeighbors = { };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
        }

        [Test]
        public void GetNeighbors_AllAccessible_InMiddle_Odd_Test()
        {
            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 3)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopLeft_Odd_Test()
        {
            // Arrange
            AHexGrid grid = CommonTestsHelper.CreateEmptyGridForTest(6, 6);
            Cell cellToGetTheNeighborsFrom = grid.Cells[new Vector2Int(0, 5)];
            Cell[] expectedNeighbors =
            {
                grid.Cells[new Vector2Int(0, 4)],
                grid.Cells[new Vector2Int(1, 4)],
                grid.Cells[new Vector2Int(1, 5)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopRight_Odd_Test()
        {
            // Arrange
            AHexGrid grid = CommonTestsHelper.CreateEmptyGridForTest(5, 4);
            Cell cellToGetTheNeighborsFrom = grid.Cells[new Vector2Int(4, 3)];
            Cell[] expectedNeighbors =
            {
                grid.Cells[new Vector2Int(4, 2)],
                grid.Cells[new Vector2Int(3, 3)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_SomeInaccessible_Odd_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.LOW,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.LOW,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_SomeHeightInaccessible_Odd_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_LowHeightDifference_Odd_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.MEDIUM,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.MEDIUM,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 3)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_Isolated_Odd_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(4, 4)].Setup(new Vector2Int(4, 4), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 2)].Setup(new Vector2Int(3, 2), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(4, 2)].Setup(new Vector2Int(4, 2), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(3, 4)].Setup(new Vector2Int(3, 4), ECellHeight.MEDIUM,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);


            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors = { };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, includeInaccessibleNeighbors: false, includeHeightInaccessibleNeighbors: false, includeOccupiedNeighbors: false);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
        }

        [Test]
        public void GetNeighbors_IncludeInaccessible_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.LOW,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.LOW,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 3)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, true);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_IncludeHeightInaccessible_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 3)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, false, true);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }

        [Test]
        public void GetNeighbors_IncludeHeightInaccessibleAndInaccessible_Test()
        {
            _grid.Cells[new Vector2Int(4, 3)].Setup(new Vector2Int(4, 3), ECellHeight.HIGH,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            _grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.MEDIUM,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell cellToGetTheNeighborsFrom = _grid.Cells[new Vector2Int(3, 3)];
            Cell[] expectedNeighbors =
            {
                _grid.Cells[new Vector2Int(4, 3)],
                _grid.Cells[new Vector2Int(2, 3)],
                _grid.Cells[new Vector2Int(4, 4)],
                _grid.Cells[new Vector2Int(3, 2)],
                _grid.Cells[new Vector2Int(4, 2)],
                _grid.Cells[new Vector2Int(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(_grid, cellToGetTheNeighborsFrom, true, true);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors) Assert.Contains(actualNeighbor, expectedNeighbors);
        }
    }
}