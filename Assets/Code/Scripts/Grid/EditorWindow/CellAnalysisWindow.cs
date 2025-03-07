using UnityEditor;
using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Grid
{
    public class CellAnalysisWindow : EditorWindow
    {
        [SerializeField] private AHexGrid Grid;
        [SerializeField] private Cell TargetCell;

        [MenuItem("Window/CellAnalysis")]
        public static void ShowWindow()
        {
            GetWindow<CellAnalysisWindow>("CellAnalysis");
        }

        private void OnGUI()
        {
            GUILayout.Label("Grid", EditorStyles.boldLabel);
            Grid = EditorGUILayout.ObjectField("Select Grid", Grid, typeof(AHexGrid), true) as AHexGrid;

            GUILayout.Label("Target Cell", EditorStyles.boldLabel);
            TargetCell = EditorGUILayout.ObjectField("Select Cell", TargetCell, typeof(Cell), true) as Cell;

            if (TargetCell != null)
            {
                if (GUILayout.Button("Analyze Neighbors"))
                {
                    CellAnalysis.AnalyzeAtCell(TargetCell, Grid);
                    CellAnalysis.PrintAnalysisWithPercentages();
                }
            }
        }
    }
}