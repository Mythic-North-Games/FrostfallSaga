using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.EditModeTests
{
    public static class CommonTestsHelper
    {
        public static HexGrid CreatePlainGridForTest(int gridWidth = 5, int gridHeight = 5)
        {
            GameObject gridGameObject = new();
            gridGameObject.AddComponent<HexGrid>();

            HexGrid grid = gridGameObject.GetComponent<HexGrid>();
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector2Int newCellCoordinates = new(x, y);
                    grid.CellsByCoordinates.Add(newCellCoordinates, CreateCellForTest(newCellCoordinates));
                }
            }
            return grid;
        }

        public static Cell CreateCellForTest(
            Vector2Int coordinates,
            ECellHeight height = ECellHeight.LOW,
            bool isAccessible = true,
            float hexGridSize = 2f
        )
        {
            GameObject cellGameObject = new();
            cellGameObject.AddComponent<Cell>();
            cellGameObject.name = "Cell[" + coordinates.x + ";" + coordinates.y + "]";

            GameObject cellVisualGameObject = new();
            cellVisualGameObject.transform.SetParent(cellGameObject.transform);
            cellVisualGameObject.AddComponent<MeshCollider>();
            cellVisualGameObject.AddComponent<CellMouseEventsController>();
            cellVisualGameObject.AddComponent<MaterialHighlightable>();

            Cell newCell = cellGameObject.GetComponent<Cell>();
            newCell.Setup(coordinates, height, isAccessible, hexGridSize);
            return newCell;
        }
    }
}