using FrostfallSaga.Grid.Cells;
using UnityEditor;
using UnityEngine;

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

                if (GUILayout.Button("Print Analyze Dict"))
                {
                    CellAnalysis.AnalyzeAtCell(TargetCell, Grid);
                    CellAnalysis.PrintAnalysisDict();
                }
            }
        }
    }
}