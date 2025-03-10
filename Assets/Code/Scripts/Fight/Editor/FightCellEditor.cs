using FrostfallSaga.Fight.FightCells;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.FFSEditor.Fight
{
    [CustomEditor(typeof(FightCell))]
    [CanEditMultipleObjects]
    public class FightCellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FightCell fightCell = (FightCell)target;

            if (GUILayout.Button("Update High")) fightCell.UpdateHeight(fightCell.Height, 0);
        }
    }
}