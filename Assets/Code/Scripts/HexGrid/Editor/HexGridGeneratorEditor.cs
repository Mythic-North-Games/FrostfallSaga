using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGridGenerator))]
public class HexGridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexGridGenerator hexGridMeshGenerator = (HexGridGenerator)target;

        if (GUILayout.Button("Generate Hex Mesh")) 
        {
            hexGridMeshGenerator.CreateHexMesh();
        }

        if (GUILayout.Button("Clear Hex Mesh"))
        {
            hexGridMeshGenerator.ClearHexGridMesh();
        }

        if (GUILayout.Button("Generate Random High"))
        {
            hexGridMeshGenerator.GenerateRandomHigh();
        }
    }
}
