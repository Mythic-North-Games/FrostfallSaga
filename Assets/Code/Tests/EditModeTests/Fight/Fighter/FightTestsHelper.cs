using System;
using System.Collections.Generic;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.KingdomToFight;
using UnityEngine;

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

        public static Fighter CreateFighter()
        {
            GameObject fighterGameObject = new();
            fighterGameObject.AddComponent<Fighter>();
            Fighter fighter = fighterGameObject.GetComponent<Fighter>();
            SetupFighterFromNonPersistingConfiguration(
                fighter,
                Resources.Load<FighterConfigurationSO>("EditModeTests/ScriptableObjects/TestFighter")
            );


            GameObject fighterEntitiesVisualGameObject = new();
            fighterEntitiesVisualGameObject.transform.SetParent(fighterGameObject.transform);
            fighterEntitiesVisualGameObject.AddComponent<EntityVisualAnimationController>();
            fighterEntitiesVisualGameObject.AddComponent<EntityVisualMovementController>();
            EntityVisualMovementController movementController = fighterEntitiesVisualGameObject.GetComponent<EntityVisualMovementController>();
            movementController.SetParentToMoveForTests(fighterGameObject);

            return fighter;
        }

        private static void SetupFighterFromNonPersistingConfiguration(Fighter fighter, FighterConfigurationSO fighterConfiguration)
        {
            fighter.Setup(
                new(
                    fighterConfiguration.name,
                    Guid.NewGuid().ToString(),
                    null,
                    fighterConfiguration.ExtractFighterStats(),
                    fighterConfiguration.DirectAttackTargeter,
                    fighterConfiguration.DirectAttackActionPointsCost,
                    fighterConfiguration.DirectAttackEffects,
                    fighterConfiguration.DirectAttackAnimation,
                    fighterConfiguration.AvailableActiveAbilities,
                    fighterConfiguration.ReceiveDamageAnimationStateName,
                    fighterConfiguration.HealSelfAnimationStateName
                )
            );
        }

        public static void SetupFighterPositionOnGrid(HexGrid grid, Fighter fighter, Vector2Int cellCoordinates)
        {
            grid.CellsByCoordinates[cellCoordinates].GetComponent<CellFightBehaviour>().Fighter = fighter;
            fighter.cell = grid.CellsByCoordinates[cellCoordinates];
        }
    }
}