using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(KingdomHexGrid))]
    public class KingdomHexGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            KingdomHexGrid hexGrid = (KingdomHexGrid)target;

            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug buttons", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Hex Mesh"))
            {
                hexGrid.ClearCells();
                hexGrid.Initialize();
                hexGrid.GenerateGrid();
                hexGrid.GenerateInterestPoints();
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
