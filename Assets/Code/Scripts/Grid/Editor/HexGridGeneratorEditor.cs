using FrostfallSaga.Grid;
using UnityEditor;
using UnityEngine;

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
                hexGridMeshGenerator.Initialize();
                hexGridMeshGenerator.GenerateCells();
            }

            else if (GUILayout.Button("Clear Hex Mesh"))
            {
                hexGridMeshGenerator.ClearCells();
            }

            else if (GUILayout.Button("Generate Random High"))
            {
                hexGridMeshGenerator.GenerateRandomHeight();
            }
        }
    }
}
