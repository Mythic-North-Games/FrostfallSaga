using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Terrain;
using NUnit.Framework;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.Grid.Cells
{
    public class GetNeighborsTests
    {
        /// <summary>
        /// AllTerrain[4] = Plain (Accessible)
        /// AllTerrain[5] = Water (NOT Accessible)
        /// </summary>
        TerrainTypeSO[] AllTerrain = Resources.LoadAll<TerrainTypeSO>("ScriptableObjects/Grid/Terrain/");
        /// <summary>
        /// AllBiome[0] = Dark Forest
        /// AllBiome[1] = Snow
        /// AllBiome[2] = Tundra
        /// AllBiome[3] = Valley
        /// </summary>        
        BiomeTypeSO[] AllBiome = Resources.LoadAll<BiomeTypeSO>("ScriptableObjects/Grid/Biome/");


        [Test]
        public void GetNeighbors_AllAccessible_InMiddle_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 2)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(2, 2)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(3, 3)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 1)],
                grid.CellsByCoordinates[new(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerBottomLeft_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(0, 0)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(1, 0)],
                grid.CellsByCoordinates[new(0, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerBottomRight_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(4, 0)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 1)],
                grid.CellsByCoordinates[new(3, 1)],
                grid.CellsByCoordinates[new(3, 0)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopLeft_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(0, 4)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(1, 4)],
                grid.CellsByCoordinates[new(0, 3)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopRight_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(4, 4)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 3)],
                grid.CellsByCoordinates[new(3, 3)],
                grid.CellsByCoordinates[new(3, 4)],
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_SomeInaccessible_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(2, 2)].Setup(new(2, 2), ECellHeight.LOW, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 1)].Setup(new(3, 1), ECellHeight.LOW, 2f, AllTerrain[5], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 2)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(3, 3)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_SomeHeightInaccessible_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(2, 2)].Setup(new(2, 2), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 1)].Setup(new(3, 1), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 2)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(3, 3)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_LowHeightDifference_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(2, 2)].Setup(new(2, 2), ECellHeight.MEDIUM, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 1)].Setup(new(3, 1), ECellHeight.MEDIUM, 2f, AllTerrain[4], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 2)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(2, 2)],
                grid.CellsByCoordinates[new(3, 1)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(3, 3)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(2, 1)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_Isolated_Even_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(2, 2)].Setup(new(2, 2), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 1)].Setup(new(3, 1), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 3)].Setup(new(3, 3), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(4, 2)].Setup(new(4, 2), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 1)].Setup(new(2, 1), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 2)];
            Cell[] expectedNeighbors = { };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
        }

        [Test]
        public void GetNeighbors_AllAccessible_InMiddle_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 3)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopLeft_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(6, 6);
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(0, 5)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(0, 4)],
                grid.CellsByCoordinates[new(1, 4)],
                grid.CellsByCoordinates[new(1, 5)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_AllAccessible_CornerTopRight_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(5, 4);
            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(4, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 3)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_SomeInaccessible_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.LOW, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.LOW, 2f, AllTerrain[5], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_SomeHeightInaccessible_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_LowHeightDifference_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.MEDIUM, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.MEDIUM, 2f, AllTerrain[4], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 3)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_Isolated_Odd_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(4, 4)].Setup(new(4, 4), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 2)].Setup(new(3, 2), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(4, 2)].Setup(new(4, 2), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(3, 4)].Setup(new(3, 4), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);


            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = { };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
        }

        [Test]
        public void GetNeighbors_IncludeInaccessible_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.LOW, 2f, AllTerrain[5], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.LOW, 2f, AllTerrain[5], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 3)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom, true);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_IncludeHeightInaccessible_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 3)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom, false, true);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }

        [Test]
        public void GetNeighbors_IncludeHeightInaccessibleAndInaccessible_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest();
            grid.CellsByCoordinates[new(4, 3)].Setup(new(4, 3), ECellHeight.HIGH, 2f, AllTerrain[4], AllBiome[3]);
            grid.CellsByCoordinates[new(2, 3)].Setup(new(2, 3), ECellHeight.MEDIUM, 2f, AllTerrain[5], AllBiome[3]);

            Cell cellToGetTheNeighborsFrom = grid.CellsByCoordinates[new(3, 3)];
            Cell[] expectedNeighbors = {
                grid.CellsByCoordinates[new(4, 3)],
                grid.CellsByCoordinates[new(2, 3)],
                grid.CellsByCoordinates[new(4, 4)],
                grid.CellsByCoordinates[new(3, 2)],
                grid.CellsByCoordinates[new(4, 2)],
                grid.CellsByCoordinates[new(3, 4)]
            };

            // Act
            Cell[] actualNeighbors = CellsNeighbors.GetNeighbors(grid, cellToGetTheNeighborsFrom, true, true);

            // Assert
            Assert.AreEqual(expectedNeighbors.Length, actualNeighbors.Length);
            foreach (Cell actualNeighbor in actualNeighbors)
            {
                Assert.Contains(actualNeighbor, expectedNeighbors);
            }
        }
    }
}
