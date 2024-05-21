using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cell))]
public class CellEditor : Editor
{
	public override void OnInspectorGUI()
	{
        DrawDefaultInspector();

        Cell cell = (Cell)target;

        if (GUILayout.Button("Update High"))
        {
            cell.OnHighChanged();
        }
    }
}