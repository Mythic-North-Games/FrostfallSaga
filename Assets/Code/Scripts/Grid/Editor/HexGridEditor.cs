using UnityEditor;
using UnityEngine;
using FrostfallSaga.Grid;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(HexGrid))]
    public class HexGridEditor : Editor
    {
        private void DrawDebugCellNames()
        {
            HexGrid hexGrid = (HexGrid)target;

            for (int z = 0; z < hexGrid.Height; z++)
            {
                for (int x = 0; x < hexGrid.Width; x++)
                {
                    Vector3 centerPosition = HexMetrics.Center(hexGrid.HexSize, x, z, hexGrid.HexOrientation);
                    int centerX = x;
                    int centerZ = z;
                    Handles.Label(centerPosition + Vector3.forward, $"Hex[X:{centerX}, Z:{centerZ}]");
                }
            }
        }
    }
}
