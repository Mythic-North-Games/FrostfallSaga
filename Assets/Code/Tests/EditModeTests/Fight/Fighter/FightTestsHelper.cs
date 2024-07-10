using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.EditModeTests.FightTests
{
    public static class FightTestsHelper
    {
        public static HexGrid CreatePlainFightGrid(int width = 5, int height = 5)
        {
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(width, height);
            foreach (KeyValuePair<Vector2Int, Cell> coordsToCell in grid.CellsByCoordinates)
            {
                coordsToCell.Value.gameObject.AddComponent<CellFightBehaviour>();
            }
            return grid;
        }

        public static Fighter CreateFighter(FighterConfigurationSO fighterConfiguration = null)
        {
            GameObject fighterGameObject = new();
            fighterGameObject.AddComponent<Fighter>();
            Fighter fighter = fighterGameObject.GetComponent<Fighter>();

            GameObject fighterEntitiesVisualGameObject = new();
            fighterEntitiesVisualGameObject.transform.SetParent(fighterGameObject.transform);
            fighterEntitiesVisualGameObject.AddComponent<EntityVisualAnimationController>();
            fighterEntitiesVisualGameObject.AddComponent<EntityVisualMovementController>();
            EntityVisualMovementController movementController = fighterEntitiesVisualGameObject.GetComponent<EntityVisualMovementController>();
            movementController.SetParentToMoveForTests(fighterGameObject);

            fighter.SetFighterConfigurationForTests(
                fighterConfiguration != null ?
                    fighterConfiguration :
                    Resources.Load<FighterConfigurationSO>("EditModeTests/ScriptableObjects/TestFighter")
            );
            return fighter;
        }

        public static void SetupFighterPositionOnGrid(HexGrid grid, Fighter fighter, Vector2Int cellCoordinates)
        {
            grid.CellsByCoordinates[cellCoordinates].GetComponent<CellFightBehaviour>().Fighter = fighter;
            fighter.cell = grid.CellsByCoordinates[cellCoordinates];
        }
    }
}