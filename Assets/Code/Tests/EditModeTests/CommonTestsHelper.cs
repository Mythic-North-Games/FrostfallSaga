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
        private static readonly GameObject HexGridKingdomPrefab = Resources.Load<GameObject>("EditModeTests/PrefabsTests/HexGridKingdomTest");
        private static readonly GameObject HexGridFightPrefab = Resources.Load<GameObject>("EditModeTests/PrefabsTests/HexGridFightTest");
        public static readonly BiomeTypeSO DefaultBiome = Resources.Load<BiomeTypeSO>("EditModeTests/ScriptableObjects/TestBiome");
        public static readonly TerrainTypeSO AccessibleTerrain = Resources.Load<TerrainTypeSO>("EditModeTests/ScriptableObjects/TestTerrainTypeAccessible");
        public static readonly TerrainTypeSO InaccessibleTerrain = Resources.Load<TerrainTypeSO>("EditModeTests/ScriptableObjects/TestTerrainTypeInaccessible");

        /// <summary>
        /// Create a grid using the GridFactory for testing purposes.
        /// </summary>
        public static HexGrid CreatePlainGridForTest(EGridType eGridType, int gridWidth = 5, int gridHeight = 5)
        {
            GameObject gameObject = new();
            if (eGridType == EGridType.KINGDOM)
            {
                gameObject = Object.Instantiate(HexGridKingdomPrefab);
            }
            else if (eGridType == EGridType.FIGHT)
            {
                gameObject = Object.Instantiate(HexGridFightPrefab);
            }
            else
            {
                Debug.LogError($"ERROR : {eGridType} is invalid");
            }

            HexGrid grid = gameObject.GetComponent<HexGrid>();
            grid.Width = gridWidth != 5 ? gridWidth : grid.Width;
            grid.Height = gridHeight != 5 ? gridHeight : grid.Height;
            IGridGenerator gridGenerator = GridFactory.CreateGridGenerator(
                eGridType,
                grid.Width,
                grid.Height,
                new[] { DefaultBiome },
                grid.transform,
                0.2f,
                000_000_000
            );
            grid.Cells = gridGenerator.GenerateGrid(grid.HexPrefab.GetComponent<Cell>(), grid.HexSize);
            foreach (Cell cell in grid.Cells.Values)
            {
                cell.SetTerrain(AccessibleTerrain);
            }
            return grid;
        }

        /// <summary>
        /// Creates a single test cell (KingdomCell or FightCell) for testing purposes.
        /// </summary>
        public static Cell CreateCellForTest(
            Vector2Int coordinates,
            bool fightCell = false,
            ECellHeight height = ECellHeight.LOW,
            float hexGridSize = 2f
        )
        {
            GameObject cellGameObject = new();
            if (fightCell)
            {
                cellGameObject.AddComponent<FightCell>();
            }
            else
            {
                cellGameObject.AddComponent<KingdomCell>();
            }
            cellGameObject.name = "Cell[" + coordinates.x + ";" + coordinates.y + "]";

            GameObject cellVisualGameObject = new();
            cellVisualGameObject.transform.SetParent(cellGameObject.transform);
            cellVisualGameObject.AddComponent<MeshCollider>();
            cellVisualGameObject.AddComponent<CellMouseEventsController>();
            cellVisualGameObject.AddComponent<MaterialHighlightable>();

            Cell newCell = cellGameObject.GetComponent<Cell>();
            newCell.Setup(coordinates, height, hexGridSize, AccessibleTerrain, DefaultBiome);

            return newCell;
        }
    }
}