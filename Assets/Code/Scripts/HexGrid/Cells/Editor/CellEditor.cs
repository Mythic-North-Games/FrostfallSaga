using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
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