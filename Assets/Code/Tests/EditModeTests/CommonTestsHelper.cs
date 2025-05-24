using System.Collections.Generic;
using FrostfallSaga.Fight;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.EditModeTests
{
    public static class CommonTestsHelper
    {
        private static readonly GameObject HexGridKingdomPrefabTest =
            Resources.Load<GameObject>("EditModeTests/PrefabsTests/HexGridKingdomTest");

        private static readonly GameObject HexGridFightPrefabTest =
            Resources.Load<GameObject>("EditModeTests/PrefabsTests/HexGridFightTest");

        public static readonly BiomeTypeSO DefaultBiomeTest =
            Resources.Load<BiomeTypeSO>("EditModeTests/ScriptableObjects/TestBiome");

        public static readonly TerrainTypeSO AccessibleTerrain =
            Resources.Load<TerrainTypeSO>("EditModeTests/ScriptableObjects/TestTerrainTypeAccessible");

        public static readonly TerrainTypeSO InaccessibleTerrain =
            Resources.Load<TerrainTypeSO>("EditModeTests/ScriptableObjects/TestTerrainTypeInaccessible");


        public static KingdomHexGrid CreateEmptyGridForTest(int width = 5, int height = 5)
        {
            GameObject gameObject = Object.Instantiate(HexGridKingdomPrefabTest);
            KingdomHexGrid grid = gameObject.GetComponent<KingdomHexGrid>();
            grid.Width = width;
            grid.Height = height;
            grid.AvailableBiomes = new[] { DefaultBiomeTest };
            grid.Initialize();

            grid.Cells = new Dictionary<Vector2Int, Cell>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int coord = new(x, y);
                    CreateCellForTest(coord, grid);
                }
            }

            return grid;
        }


        /// <summary>
        ///     Create a grid using the GridFactory for testing purposes.
        /// </summary>
        public static FightHexGrid CreatePlainGridFightForTest(int gridWidth = 5, int gridHeight = 5)
        {
            GameObject gameObject = Object.Instantiate(HexGridFightPrefabTest);
            FightHexGrid grid = gameObject.GetComponent<FightHexGrid>();
            grid.Width = gridWidth != 5 ? gridWidth : grid.Width;
            grid.Height = gridHeight != 5 ? gridHeight : grid.Height;
            grid.AvailableBiomes = new[] { DefaultBiomeTest };
            grid.Initialize();
            grid.GenerateGridForTests();
            foreach (Cell cell in grid.Cells.Values) cell.SetTerrain(AccessibleTerrain);
            return grid;
        }

        /// <summary>
        ///     Creates a single test cell (KingdomCell or FightCell) for testing purposes.
        /// </summary>
        public static Cell CreateCellForTest(
            Vector2Int coordinates,
            AHexGrid parentGrid,
            bool fightCell = false,
            ECellHeight height = ECellHeight.LOW
        )
        {
            GameObject cellGameObject = new();
            if (fightCell)
                cellGameObject.AddComponent<FightCell>();
            else
                cellGameObject.AddComponent<KingdomCell>();
            cellGameObject.name = "Cell[" + coordinates.x + ";" + coordinates.y + "]";

            GameObject cellVisualGameObject = new();
            cellVisualGameObject.transform.SetParent(cellGameObject.transform);
            cellVisualGameObject.AddComponent<MeshCollider>();
            cellVisualGameObject.AddComponent<MaterialHighlightable>();

            Cell newCell = cellGameObject.GetComponent<Cell>();
            newCell.transform.SetParent(parentGrid.transform);

            newCell.Initialize();
            newCell.Setup(coordinates, height, AccessibleTerrain, DefaultBiomeTest);

            parentGrid.Cells[coordinates] = newCell;

            return newCell;
        }
    }
}