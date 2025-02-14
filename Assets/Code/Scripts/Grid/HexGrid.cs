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

        public override string ToString()
        {
            return $"HexGrid: \n" +
                   $"- Width: {Width}\n" +
                   $"- Height: {Height}\n" +
                   $"- HexSize: {HexSize}\n" +
                   $"- HexPrefab: {(HexPrefab != null ? HexPrefab.name : "None")}\n" +
                   $"- Total Cells: {CellsByCoordinates.Count}\n" +
                   $"- Cells Info: \n" +
                   $"{string.Join("\n", CellsByCoordinates.Select(kvp => $"  * Coordinates: {kvp.Key}, Cell: {kvp.Value}"))}";
        }
    }
}
