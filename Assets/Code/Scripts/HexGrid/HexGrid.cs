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

        public Cell[] GetCells()
        {
            return GetComponentsInChildren<Cell>();
        }

        private void OnDrawGizmos()
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
