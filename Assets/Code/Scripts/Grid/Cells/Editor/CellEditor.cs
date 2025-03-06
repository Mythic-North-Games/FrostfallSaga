using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(Cell))]
    [CanEditMultipleObjects]
    public class CellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Cell cell = (Cell)target;

            if (GUILayout.Button("Update High"))
            {
                cell.UpdateHeight(cell.Height,0);
            }
            else if (GUILayout.Button("Get Neighbors"))
            {
                Cell[] cellList = CellsNeighbors.GetNeighbors(cell.GetComponentInParent<AHexGrid>(), cell);
                foreach (var item in cellList)
                {
                    Debug.Log(item.transform.name);
                }
            }
        }
    }
}