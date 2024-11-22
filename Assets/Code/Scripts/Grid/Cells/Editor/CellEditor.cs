using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using UnityEditor;
using UnityEngine;

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
                cell.UpdateHeight(cell.Height);
            }
            else if (GUILayout.Button("Get Neighbors"))
            {
                Cell[] cellList = CellsNeighbors.GetNeighbors(cell.GetComponentInParent<HexGrid>(), cell);
                foreach (var item in cellList)
                {
                    Debug.Log(item.transform.name);
                }
            }
        }
    }
}