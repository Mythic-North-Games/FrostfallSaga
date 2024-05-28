using UnityEditor;
using UnityEngine;
using FrostfallSaga.Grid;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(GridCellsGenerator))]
    public class HexGridGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridCellsGenerator hexGridMeshGenerator = (GridCellsGenerator)target;

            if (GUILayout.Button("Generate Hex Mesh"))
            {
                hexGridMeshGenerator.GenerateCells();
            }

            if (GUILayout.Button("Clear Hex Mesh"))
            {
                hexGridMeshGenerator.ClearCells();
            }

            if (GUILayout.Button("Generate Random High"))
            {
                hexGridMeshGenerator.GenerateRandomHeight();
            }
        }
    }
}
