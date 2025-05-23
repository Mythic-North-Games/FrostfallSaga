using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightersGenerator
    {
        [SerializeField] private EntityConfigurationSO[] _devAlliesConfs;
        [SerializeField] private EntityConfigurationSO[] _devEnemiesConfs;

        public FightersGenerator(EntityConfigurationSO[] devAlliesConfs, EntityConfigurationSO[] devEnemiesConfs)
        {
            _devAlliesConfs = devAlliesConfs;
            _devEnemiesConfs = devEnemiesConfs;
        }

        public KeyValuePair<Fighter[], Fighter[]> GenerateFighters()
        {
            PreFightData preFightData = GameStateManager.Instance.GetPreFightData();

            // Adjust fighter to build based on pre fight data or dev configuration
            EntityConfigurationSO[] alliesFighterConf =
                preFightData.alliesEntityConf != null && preFightData.alliesEntityConf.Length > 0
                    ? preFightData.alliesEntityConf
                    : _devAlliesConfs;

            // Heal allies if from dev
            if (alliesFighterConf == _devAlliesConfs)
            {
                alliesFighterConf.ToList().ForEach(allyFighterConf =>
                {
                    PersistedFighterConfigurationSO fighterConfiguration =
                        allyFighterConf.FighterConfiguration as PersistedFighterConfigurationSO;
                    fighterConfiguration.SetHealth(fighterConfiguration.MaxHealth);
                });
            }

            KeyValuePair<string, EntityConfigurationSO>[] enemiesFighterConf =
                preFightData.enemiesEntityConf != null && preFightData.enemiesEntityConf.Length > 0
                    ? preFightData.enemiesEntityConf
                    : BuildDevFighterConfMapping(_devEnemiesConfs);

            List<Fighter> allies = new();
            alliesFighterConf.ToList().ForEach(allyFighterConf =>
                allies.Add(SpawnAndSetupFighter(allyFighterConf))
            );

            List<Fighter> enemies = new();
            enemiesFighterConf.ToList().ForEach(enemyFighterConf =>
                enemies.Add(
                    SpawnAndSetupFighter(
                        enemyFighterConf.Value,
                        enemyFighterConf.Key,
                        $"{enemies.Count}"
                    )
                )
            );
            return new KeyValuePair<Fighter[], Fighter[]>(allies.ToArray(), enemies.ToArray());
        }

        private Fighter SpawnAndSetupFighter(
            EntityConfigurationSO entityConfiguration,
            string sessionId = null,
            string nameSuffix = ""
        )
        {
            // Spawn fighter game object
            FighterConfigurationSO fighterConfiguration = entityConfiguration.FighterConfiguration;
            GameObject fighterGameObject =
                WorldGameObjectInstantiator.Instance.Instantiate(fighterConfiguration.FighterPrefab);
            fighterGameObject.name = new string($"{entityConfiguration.Name}{nameSuffix}");

            // Setup spawned fighter
            Fighter fighter = fighterGameObject.GetComponent<Fighter>();
            if (entityConfiguration.FighterConfiguration is PersistedFighterConfigurationSO)
                SetupAllyFighter(fighter, entityConfiguration, sessionId);
            else
                SetupEnemyFighter(fighter, entityConfiguration, sessionId);

            return fighter;
        }

        private void SetupEnemyFighter(Fighter enemyFighterToSetup,
            EntityConfigurationSO entityConfiguration,
            string sessionId)
        {
            FighterConfigurationSO fighterConfiguration = entityConfiguration.FighterConfiguration;
            enemyFighterToSetup.Setup(
                entityConfiguration,
                fighterConfiguration,
                equippedActiveAbilities: ChooseEnemyActiveAbilities(fighterConfiguration),
                equippedPassiveAbilities: ChooseEnemyPassiveAbilities(fighterConfiguration),
                inventory: GenerateEnemyFightInventory(fighterConfiguration),
                sessionId: sessionId
            );
        }

        private static void SetupAllyFighter(
            Fighter allyFighterToSetup,
            EntityConfigurationSO entityConf,
            string sessionId
        )
        {
            PersistedFighterConfigurationSO fighterConf = entityConf.FighterConfiguration as PersistedFighterConfigurationSO;
            ActiveAbilitySO[] activeAbilities = fighterConf.EquippedActiveAbilities
                .Select(activeAbility => activeAbility as ActiveAbilitySO)
                .ToArray();
            PassiveAbilitySO[] passiveAbilities = fighterConf.EquippedPassiveAbilities
                .Select(passiveAbility => passiveAbility as PassiveAbilitySO)
                .ToArray();

            allyFighterToSetup.Setup(
                entityConfiguration: entityConf,
                fighterConfiguration: fighterConf,
                equippedActiveAbilities: activeAbilities,
                equippedPassiveAbilities: passiveAbilities,
                inventory: fighterConf.Inventory,
                sessionId
            );
        }

        private static Inventory GenerateEnemyFightInventory(FighterConfigurationSO fighterConfiguration)
        {
            Inventory inventory = new();

            // Get the start items for the enemy inventory
            ItemSO[] startItems = ComputeEnemyInventoryStartItems(fighterConfiguration.AvailableItems);
            foreach (ItemSO item in startItems)
            {
                inventory.AddItemAndEquipIfPossible(item);
            }

            if (inventory.WeaponSlot.IsEmpty())
            {
                Debug.LogWarning("Enemy inventory is missing a weapon. Default weapon will be equipped.");
                inventory.EquipEquipment(Resources.Load<WeaponSO>(Inventory.DefaultWeaponResourcePath), isFromBag: false);
            }

            return inventory;
        }

        /// <summary>
        /// Compute the enemy inventory at fight start based on the available items and their apparition chances.
        /// </summary>
        /// <param name="availableItems">The available items and their apparition chances.</param>
        /// <returns>The computed enemy start items.</returns>
        private static ItemSO[] ComputeEnemyInventoryStartItems(
            Dictionary<ItemSO, (float apparitionChance, int maxApparitionCount)> availableItems
        )
        {
            List<ItemSO> items = new();
            foreach (KeyValuePair<ItemSO, (float apparitionChance, int maxApparitionCount)> item in availableItems)
            {
                ItemSO possibleItem = item.Key;
                float includeInInventoryChance = item.Value.apparitionChance;
                int maxPossibleItemCount = item.Value.maxApparitionCount;

                for (int i = 0; i < maxPossibleItemCount; i++)
                {
                    if (Randomizer.GetBooleanOnChance(includeInInventoryChance))
                    {
                        items.Add(possibleItem);
                    }
                }
            }

            return items.ToArray();
        }

        private static ActiveAbilitySO[] ChooseEnemyActiveAbilities(FighterConfigurationSO fighterConfiguration)
        {
            ActiveAbilitySO[] availableActiveAbilities = Array.ConvertAll(
                fighterConfiguration.UnlockedActiveAbilities.ToArray(),
                activeAbility => activeAbility as ActiveAbilitySO
            );
            if (availableActiveAbilities.Length == 0) return Array.Empty<ActiveAbilitySO>();

            return Randomizer.GetRandomUniqueElementsFromArray(
                availableActiveAbilities,
                fighterConfiguration.ActiveAbilitiesCapacity
            );
        }

        private static PassiveAbilitySO[] ChooseEnemyPassiveAbilities(FighterConfigurationSO fighterConfiguration)
        {
            PassiveAbilitySO[] availablePassiveAbilities = Array.ConvertAll(
                fighterConfiguration.UnlockedPassiveAbilities.ToArray(),
                passiveAbility => passiveAbility as PassiveAbilitySO
            );
            if (availablePassiveAbilities.Length == 0) return Array.Empty<PassiveAbilitySO>();

            return Randomizer.GetRandomUniqueElementsFromArray(
                availablePassiveAbilities,
                fighterConfiguration.PassiveAbilitiesCapacity
            );
        }

        private static KeyValuePair<string, EntityConfigurationSO>[] BuildDevFighterConfMapping(
            EntityConfigurationSO[] devEntityConfs)
        {
            return devEntityConfs.Select(
                devEntityConf => new KeyValuePair<string, EntityConfigurationSO>(null, devEntityConf)
            ).ToArray();
        }
    }
}