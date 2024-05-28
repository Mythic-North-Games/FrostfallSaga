using UnityEditor;
using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(Cell))]
    public class CellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Cell cell = (Cell)target;

            if (GUILayout.Button("Update High"))
            {
                cell.UpdateHeight(cell.CellHeight);
            }
        }
    }
}