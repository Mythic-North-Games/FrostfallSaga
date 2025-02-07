using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Grid
{
    [CustomEditor(typeof(FightCell))]
    [CanEditMultipleObjects]
    public class FightCellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Cell cell = (FightCell)target;

            if (GUILayout.Button("Update High"))
            {
                cell.UpdateHeight(cell.Data.Height, 0f);
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