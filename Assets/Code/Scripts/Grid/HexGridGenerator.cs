using UnityEngine;
using FrostfallSaga.Grid.Cells;
using System.Linq;
using FrostfallSaga.Core;
using System;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Expose methods for generating cells inside a given grid.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class GridCellsGenerator : MonoBehaviour
    {
        [field: SerializeField] public HexGrid HexGrid { get; private set; }
        [SerializeField] private Material AlternativeMaterial;
        [field: SerializeField, Header("Biome caracteristics"), Tooltip("Biome's type")] public BiomeTypeSO BiomeType { get; private set; }

        private void Awake()
        {
            if (HexGrid == null)
            {
                HexGrid = GetComponentInParent<HexGrid>();
            }
            if (HexGrid == null)
            {
                Debug.LogError("HexGridGenerator could not find HexGrid component in its parent or itself");
            }
        }

        /// <summary>
        /// Clears existing cells of the grid if any then generate new ones.
        /// </summary>
        public void GenerateCells()
        {
            // Don't generate cells over existing cells
            ClearCells();

            Quaternion rotation = Quaternion.identity;
            if (HexGrid.HexOrientation.Equals(ECellOrientation.FlatTop))
            {
                rotation = Quaternion.Euler(new Vector3(0f, -30f, 0f));
            }

            for (int z = 0; z < HexGrid.Height; z++)
            {
                for (int x = 0; x < HexGrid.Width; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexGrid.HexSize, x, z, HexGrid.HexOrientation);
                    GameObject newHex = Instantiate(HexGrid.HexPrefab, centerPosition, rotation, HexGrid.transform);
                    SetupCellForInstanciatedCellPrefab(newHex, x, z);
                }
            }
            HexGrid.FindAndSetCellsByCoordinates();
        }

        /// <summary>
        /// Destroys all the cells of the grid.
        /// </summary>
        public void ClearCells()
        {

            GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
            if (cells != null)
            {
                foreach (GameObject cell in cells)
                {
                    DestroyImmediate(cell);
                }
            }
        }

        /// <summary>
        /// Updates the height of existing cells with a random height.
        /// </summary>
        public void GenerateRandomHeight()
        {
            int childCount = HexGrid.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Cell cell = HexGrid.transform.GetChild(i).GetComponent<Cell>();
                ECellHeight randomCellHeight = (ECellHeight) Randomizer.GetRandomIntBetween(-1, 2);
                cell.UpdateHeight(randomCellHeight);
            }
        }

        private void SetupCellForInstanciatedCellPrefab(GameObject cellPrefab, int x, int z)
        {
            cellPrefab.transform.name = "Cell[" + x + ";" + z + "]";
            Cell newCell = cellPrefab.GetComponent<Cell>();
            TerrainTypeSO _terrain = Randomizer.GetRandomElementFromArray(BiomeType.TerrainTypeSO);
            newCell.Setup(new Vector2Int(x, z), ECellHeight.LOW, true, HexGrid.HexSize, _terrain);
            newCell.HighlightController.SetupInitialMaterial(_terrain.CellMaterial);
            newCell.HighlightController.UpdateCurrentDefaultMaterial(_terrain.CellMaterial);
            newCell.HighlightController.ResetToInitialMaterial();
        }
    }
}
