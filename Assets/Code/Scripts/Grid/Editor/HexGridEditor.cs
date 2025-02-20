using FrostfallSaga.Grid;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(HexGrid))]
    public class HexGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            HexGrid hexGrid = (HexGrid)target;

            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug buttons", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Hex Mesh"))
            {
                hexGrid.Initialize();
            }

            else if (GUILayout.Button("Clear Hex Mesh"))
            {
                hexGrid.ClearCells();
            }

            else if (GUILayout.Button("Generate Random High"))
            {
                hexGrid.GenerateRandomHeight();
            }
            else if (GUILayout.Button("ToString"))
            {
                Debug.Log(hexGrid.ToString());
            }
        }
    }
}
