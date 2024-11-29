using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    /// Represents a grid of hexagonal cells.
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        [field: SerializeField] public int Width { get; private set; }
        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public float HexSize { get; private set; }
        [field: SerializeField] public GameObject HexPrefab { get; private set; }
        [field: SerializeField] public ECellOrientation HexOrientation { get; private set; }
        public Dictionary<Vector2Int, Cell> CellsByCoordinates { get; private set; } = new();

        /// <summary>
        /// Retrieves all the Cell components that are children of the current GameObject.
        /// This method returns an array of Cell objects, allowing access to all cells 
        /// in the grid structure for further processing or manipulation.
        /// </summary>
        /// <returns>An array of Cell components found in the children of the current GameObject.</returns>
        public Cell[] GetCells()
        {
            return GetComponentsInChildren<Cell>();
        }

        /// <summary>
        /// Finds all the cells in the grid and stores them in a dictionary (`CellsByCoordinates`) 
        /// with their coordinates as the key. It clears any existing entries and then iterates 
        /// through the cells, adding each one to the dictionary for efficient lookup by coordinates.
        /// </summary>
        public void FindAndSetCellsByCoordinates()
        {
            CellsByCoordinates.Clear();
            GetCells().ToList().ForEach(cell => CellsByCoordinates.Add(cell.Coordinates, cell));
        }

        /// <summary>
        /// Initializes the component by calling the method to find and set cells by their coordinates.
        /// This ensures that the cells are properly organized and accessible as soon as the script is loaded.
        /// </summary>
        private void Awake()
        {
            if (!AreGridDimensionsValid())
            {
                Debug.LogError("HexGrid has invalid dimensions, grid initialization aborted.");
                return;
            }

            FindAndSetCellsByCoordinates();
        }

        /// <summary>
        /// Verifies that the grid dimensions (Width and Height) are valid.
        /// </summary>
        /// <returns>True if the dimensions are valid; otherwise, false.</returns>
        private bool AreGridDimensionsValid()
        {
            if (Width <= 0 || Height <= 0)
            {
                Debug.LogError($"Invalid grid dimensions: Width ({Width}) and Height ({Height}) must both be greater than zero.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Draws the grid cells in the Unity Editor using Gizmos for visualization. 
        /// For each cell in the grid, it calculates the center position and then draws lines 
        /// to represent the hexagonal shape by connecting the corners. This is useful for debugging 
        /// and visualizing the grid layout during development.
        /// </summary>
#pragma warning disable IDE0051 // Supprimer les membres privés non utilisés
        private void DrawCellsGuizmos()
#pragma warning restore IDE0051 // Supprimer les membres privés non utilisés
        {
            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(HexSize, x, z, HexOrientation) + transform.position;

                    for (int s = 0; s < HexMetrics.Corners(HexSize, HexOrientation).Length; s++)
                    {
                        Gizmos.DrawLine(
                            centerPosition + HexMetrics.Corners(HexSize, HexOrientation)[s % 6],
                            centerPosition + HexMetrics.Corners(HexSize, HexOrientation)[(s + 1) % 6]
                         );
                    }
                }
            }
        }
    }
}
