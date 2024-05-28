using UnityEditor;
using UnityEngine;
using FrostfallSaga.Grid;
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
            else if (GUILayout.Button("Get Neighbors"))
            {
                Cell[] cellList = cell.GetNeighbors(cell.GetComponentInParent<HexGrid>());
                foreach (var item in cellList)
                {
                    Debug.Log(item.transform.name);
                }
            }
        }
    }
}