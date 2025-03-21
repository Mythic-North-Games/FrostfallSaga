using FrostfallSaga.Grid.Cells;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class CellAnalysisWindow : EditorWindow
    {
        [SerializeField] private AHexGrid grid;
        [SerializeField] private Cell targetCell;

        private void OnGUI()
        {
            GUILayout.Label("Grid", EditorStyles.boldLabel);
            grid = EditorGUILayout.ObjectField("Select Grid", grid, typeof(AHexGrid), true) as AHexGrid;

            GUILayout.Label("Target Cell", EditorStyles.boldLabel);
            targetCell = EditorGUILayout.ObjectField("Select Cell", targetCell, typeof(Cell), true) as Cell;

            if (!targetCell) return;
            if (GUILayout.Button("Analyze Neighbors"))
                CellAnalysis.AnalyzeAtCell(targetCell, grid);

            if (!GUILayout.Button("Print Analyze Dict")) return;
            CellAnalysis.PrintAnalysisDict();
        }

        [MenuItem("Window/CellAnalysis")]
        public static void ShowWindow()
        {
            GetWindow<CellAnalysisWindow>("CellAnalysis");
        }
    }
}