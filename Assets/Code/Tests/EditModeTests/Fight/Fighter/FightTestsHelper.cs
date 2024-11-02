using System;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.KingdomToFight;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.EditModeTests.FightTests
{
    public static class FightTestsHelper
    {
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
                    fighterConfiguration.FighterClass,
                    fighterConfiguration.PersonalityTrait,
                    fighterConfiguration.DirectAttackTargeter,
                    fighterConfiguration.DirectAttackActionPointsCost,
                    fighterConfiguration.DirectAttackEffects,
                    fighterConfiguration.DirectAttackAnimation,
                    fighterConfiguration.AvailableActiveAbilities,
                    fighterConfiguration.ReceiveDamageAnimationName,
                    fighterConfiguration.HealSelfAnimationName,
                    fighterConfiguration.ReduceStatAnimationName,
                    fighterConfiguration.IncreaseStatAnimationName
                )
            );
        }

        public static void SetupFighterPositionOnGrid(HexGrid grid, Fighter fighter, Vector2Int cellCoordinates)
        {
            ((FightCell)grid.CellsByCoordinates[cellCoordinates]).SetFighter(fighter);
            fighter.cell = (FightCell)grid.CellsByCoordinates[cellCoordinates];
        }
    }
}