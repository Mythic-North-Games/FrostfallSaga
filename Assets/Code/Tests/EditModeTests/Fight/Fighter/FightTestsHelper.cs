using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Grid;
using FrostfallSaga.InventorySystem;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Fight.Abilities;
using System;

namespace FrostfallSaga.EditModeTests.FightTests
{
    public static class FightTestsHelper
    {
        private static readonly string TEST_ENTITY_CONF_RESOURCE_PATH = "EditModeTests/ScriptableObjects/TestEntityConfiguration";

        public static Fighter CreateFighter()
        {
            GameObject fighterGameObject = new();
            fighterGameObject.AddComponent<Fighter>();
            Fighter fighter = fighterGameObject.GetComponent<Fighter>();
            SetupFighterFromNonPersistingConfiguration(
                fighter,
                Resources.Load<EntityConfigurationSO>(TEST_ENTITY_CONF_RESOURCE_PATH)
            );

            GameObject fighterEntitiesVisualGameObject = new();
            fighterEntitiesVisualGameObject.transform.SetParent(fighterGameObject.transform);
            fighterEntitiesVisualGameObject.AddComponent<EntityVisualAnimationController>();
            fighterEntitiesVisualGameObject.AddComponent<EntityVisualMovementController>();
            EntityVisualMovementController movementController = fighterEntitiesVisualGameObject.GetComponent<EntityVisualMovementController>();
            movementController.SetParentToMoveForTests(fighterGameObject);

            return fighter;
        }

        private static void SetupFighterFromNonPersistingConfiguration(Fighter fighter, EntityConfigurationSO entityConfiguration)
        {
            Inventory testInventory = new();
            testInventory.WeaponSlot.AddItem(Resources.Load<WeaponSO>("EditModeTests/ScriptableObjects/TestWeapon"));

            ActiveAbilitySO[] activeAbilities = Array.ConvertAll(
                entityConfiguration.FighterConfiguration.AvailableActiveAbilities,
                activeAbility => activeAbility as ActiveAbilitySO
            );
            PassiveAbilitySO[] passiveAbilities = Array.ConvertAll(
                entityConfiguration.FighterConfiguration.AvailablePassiveAbilities,
                passiveAbility => passiveAbility as PassiveAbilitySO
            );

            fighter.Setup(
                entityConfiguration,
                entityConfiguration.FighterConfiguration,
                equippedActiveAbilities: activeAbilities,
                equippedPassiveAbilities: passiveAbilities,
                inventory: testInventory,
                sessionId: null
            );
        }

        public static void SetupFighterPositionOnGrid(HexGrid grid, Fighter fighter, Vector2Int cellCoordinates)
        {
            ((FightCell)grid.Cells[cellCoordinates]).SetFighter(fighter);
            fighter.cell = (FightCell)grid.Cells[cellCoordinates];
        }
    }
}