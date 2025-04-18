using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using NUnit.Framework;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.Grid.Cells
{
    public class GetShorterPathTests
    {
        private AHexGrid grid;

        [SetUp]
        public void Setup()
        {
            grid = CommonTestsHelper.CreateEmptyGridForTest();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(grid.gameObject);
            grid = null;
        }

        #region Tests with no obstacles

        [Test]
        public void GetShorterPath_NoObstacle_StraightVertical_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(0, 4)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 1)],
                grid.Cells[new Vector2Int(0, 2)],
                grid.Cells[new Vector2Int(0, 3)],
                grid.Cells[new Vector2Int(0, 4)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_StraightHorizontal_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(3, 0)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(2, 0)],
                grid.Cells[new Vector2Int(3, 0)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_BottomLeftDiagonal_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(4, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 1)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 3)],
                grid.Cells[new Vector2Int(3, 2)],
                grid.Cells[new Vector2Int(2, 1)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_BottomRightDiagonal_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(0, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(1, 1)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 3)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(1, 1)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_TopLeftDiagonal_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(1, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 1)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(1, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_TopRightDiagonal_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(4, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 1)],
                grid.Cells[new Vector2Int(3, 2)],
                grid.Cells[new Vector2Int(2, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_BottomLeftToMiddle_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 1)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_BottomRightToMiddle_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(4, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 1)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_TopLeftToMiddle_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(0, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 3)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_NoObstacle_TopRightToMiddle_Test()
        {
            Cell startCell = grid.Cells[new Vector2Int(4, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 3)],
                grid.Cells[new Vector2Int(2, 3)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        #endregion

        #region Tests with inaccessible cell blocking

        [Test]
        public void GetShorterPath_WithInaccessibleCell_StraightVertical_Test()
        {
            grid.Cells[new Vector2Int(0, 2)].Setup(new Vector2Int(0, 2), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(0, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 1)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(0, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_StraightHorizontal_Test()
        {
            grid.Cells[new Vector2Int(2, 0)].Setup(new Vector2Int(2, 0), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(3, 0)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(1, 1)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(3, 0)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_BottomLeftDiagonal_Test()
        {
            grid.Cells[new Vector2Int(3, 3)].Setup(new Vector2Int(3, 3), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 1)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 4)],
                grid.Cells[new Vector2Int(2, 3)],
                grid.Cells[new Vector2Int(2, 2)],
                grid.Cells[new Vector2Int(2, 1)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_BottomRightDiagonal_Test()
        {
            grid.Cells[new Vector2Int(0, 3)].Setup(new Vector2Int(0, 3), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(1, 1)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 4)],
                grid.Cells[new Vector2Int(1, 3)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(1, 1)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_TopLeftDiagonal_Test()
        {
            grid.Cells[new Vector2Int(0, 1)].Setup(new Vector2Int(0, 1), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(1, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(1, 1)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(1, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_TopRightDiagonal_Test()
        {
            grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 0)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(2, 2)],
                grid.Cells[new Vector2Int(2, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_BottomLeftToMiddle_Test()
        {
            grid.Cells[new Vector2Int(1, 2)].Setup(new Vector2Int(1, 2), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(1, 1)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_BottomRightToMiddle_Test()
        {
            grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 0)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_TopLeftToMiddle_Test()
        {
            grid.Cells[new Vector2Int(1, 2)].Setup(new Vector2Int(1, 2), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 4)],
                grid.Cells[new Vector2Int(1, 3)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithInaccessibleCell_TopRightToMiddle_Test()
        {
            grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 3)],
                grid.Cells[new Vector2Int(3, 2)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        #endregion

        #region Tests with too high cell blocking

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_StraightVertical_Test()
        {
            grid.Cells[new Vector2Int(0, 2)].Setup(new Vector2Int(0, 2), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(0, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(0, 1)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(0, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_StraightHorizontal_Test()
        {
            grid.Cells[new Vector2Int(2, 0)].Setup(new Vector2Int(2, 0), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(3, 0)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(1, 1)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(3, 0)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_BottomLeftDiagonal_Test()
        {
            grid.Cells[new Vector2Int(3, 3)].Setup(new Vector2Int(3, 3), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 1)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 4)],
                grid.Cells[new Vector2Int(2, 3)],
                grid.Cells[new Vector2Int(2, 2)],
                grid.Cells[new Vector2Int(2, 1)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_BottomRightDiagonal_Test()
        {
            grid.Cells[new Vector2Int(0, 3)].Setup(new Vector2Int(0, 3), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(1, 1)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 4)],
                grid.Cells[new Vector2Int(1, 3)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(1, 1)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_TopLeftDiagonal_Test()
        {
            grid.Cells[new Vector2Int(0, 1)].Setup(new Vector2Int(0, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(1, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(1, 1)],
                grid.Cells[new Vector2Int(1, 2)],
                grid.Cells[new Vector2Int(1, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_TopRightDiagonal_Test()
        {
            grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 3)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 0)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(2, 2)],
                grid.Cells[new Vector2Int(2, 3)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_BottomLeftToMiddle_Test()
        {
            grid.Cells[new Vector2Int(1, 2)].Setup(new Vector2Int(1, 2), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(1, 1)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_BottomRightToMiddle_Test()
        {
            grid.Cells[new Vector2Int(3, 1)].Setup(new Vector2Int(3, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 0)],
                grid.Cells[new Vector2Int(2, 1)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_TopLeftToMiddle_Test()
        {
            grid.Cells[new Vector2Int(1, 2)].Setup(new Vector2Int(1, 2), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 4)],
                grid.Cells[new Vector2Int(1, 3)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_WithHeightInaccessibleCell_TopRightToMiddle_Test()
        {
            grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(4, 4)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(3, 3)],
                grid.Cells[new Vector2Int(3, 2)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        #endregion

        #region Tests with multiple inacessible cells

        [Test]
        public void GetShorterPath_WithMultipleInaccessibleCells_BottomLeftToMiddle_Test()
        {
            grid.Cells[new Vector2Int(2, 1)].Setup(new Vector2Int(2, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(1, 1)].Setup(new Vector2Int(1, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(1, 2)].Setup(new Vector2Int(1, 2), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(1, 3)].Setup(new Vector2Int(1, 3), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.MEDIUM, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath =
            {
                grid.Cells[new Vector2Int(1, 0)],
                grid.Cells[new Vector2Int(2, 0)],
                grid.Cells[new Vector2Int(3, 0)],
                grid.Cells[new Vector2Int(3, 1)],
                grid.Cells[new Vector2Int(3, 2)],
                grid.Cells[new Vector2Int(2, 2)]
            };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        [Test]
        public void GetShorterPath_TargetCellInaccessible_Test()
        {
            grid.Cells[new Vector2Int(2, 1)].Setup(new Vector2Int(2, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(1, 1)].Setup(new Vector2Int(1, 1), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(1, 2)].Setup(new Vector2Int(1, 2), ECellHeight.HIGH, 2f,
                CommonTestsHelper.AccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(1, 3)].Setup(new Vector2Int(1, 3), ECellHeight.LOW, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(2, 3)].Setup(new Vector2Int(2, 3), ECellHeight.MEDIUM, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);
            grid.Cells[new Vector2Int(3, 2)].Setup(new Vector2Int(3, 2), ECellHeight.MEDIUM, 2f,
                CommonTestsHelper.InaccessibleTerrain, CommonTestsHelper.DefaultBiomeTest);

            Cell startCell = grid.Cells[new Vector2Int(0, 0)];
            Cell targetCell = grid.Cells[new Vector2Int(2, 2)];
            Cell[] expectedShorterPath = { };

            // Act
            Cell[] actualShorterPath = CellsPathFinding.GetShorterPath(grid, startCell, targetCell);

            // Assert
            Assert.AreEqual(expectedShorterPath, actualShorterPath);
        }

        #endregion
    }
}