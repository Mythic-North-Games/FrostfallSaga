using UnityEditor;
using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Grid
{
    public class CellAnalysisWindow : EditorWindow
    {
        [SerializeField] private HexGrid Grid;
        [SerializeField] private Cell TargetCell;

        [MenuItem("Window/CellAnalysis")]
        public static void ShowWindow()
        {
            GetWindow<CellAnalysisWindow>("CellAnalysis");
        }

        private void OnGUI()
        {
            GUILayout.Label("Grid", EditorStyles.boldLabel);
            Grid = EditorGUILayout.ObjectField("Select Grid", Grid, typeof(HexGrid), true) as HexGrid;

            GUILayout.Label("Target Cell", EditorStyles.boldLabel);
            TargetCell = EditorGUILayout.ObjectField("Select Cell", TargetCell, typeof(Cell), true) as Cell;

            if (TargetCell != null)
            {
                if (GUILayout.Button("Analyze Neighbors"))
                {
                    CellAnalysis analyzer = new CellAnalysis(TargetCell, Grid);
                    analyzer.Analyze();
                    //analyzer.PrintAnalysisWithPercentages();
                    analyzer.PrintAnalysis();
                }
            }
        }
    }
}