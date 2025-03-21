using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

        private Fighter SetupEnemyFighter(
            Fighter enemyFighterToSetup,
            EntityConfigurationSO entityConfiguration,
            string sessionId
        )
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
            return enemyFighterToSetup;
        }

        private Fighter SetupAllyFighter(
            Fighter allyFighterToSetup,
            EntityConfigurationSO entityConfiguration,
            string sessionId
        )
        {
            PersistedFighterConfigurationSO fighterConfiguration =
                entityConfiguration.FighterConfiguration as PersistedFighterConfigurationSO;
            ActiveAbilitySO[] activeAbilities = Array.ConvertAll(
                fighterConfiguration.EquipedActiveAbilities,
                activeAbility => activeAbility as ActiveAbilitySO
            );
            PassiveAbilitySO[] passiveAbilities = Array.ConvertAll(
                fighterConfiguration.EquipedPassiveAbilities,
                passiveAbility => passiveAbility as PassiveAbilitySO
            );

            allyFighterToSetup.Setup(
                entityConfiguration,
                fighterConfiguration,
                activeAbilities,
                passiveAbilities,
                fighterConfiguration.Inventory,
                sessionId
            );
            return allyFighterToSetup;
        }

        private Inventory GenerateEnemyFightInventory(FighterConfigurationSO fighterConfiguration)
        {
            Inventory inventory = new();

            // Get the start items for the enemy inventory
            ItemSO[] startItems = ComputeEnemyInventoryStartItems(
                SElementToValue<ItemSO, SElementToValue<float, int>>.GetDictionaryFromArray(fighterConfiguration.AvailableItems)
            );
            foreach (ItemSO item in startItems)
            {
                inventory.AddItemAndEquipIfPossible(item);
            }

            if (inventory.WeaponSlot.IsEmpty())
            {
                Debug.LogWarning("Enemy inventory is missing a weapon. Default weapon will be equipped.");
                inventory.EquipItem(Resources.Load<WeaponSO>(Inventory.DefaultWeaponResourcePath));
            }
            return inventory;
        }

        /// <summary>
        /// Compute the enemy inventory at fight start based on the available items and their apparition chances.
        /// </summary>
        /// <param name="availableItems">The available items and their apparition chances.</param>
        /// <returns>The computed enemy start items.</returns>
        private ItemSO[] ComputeEnemyInventoryStartItems(Dictionary<ItemSO, SElementToValue<float, int>> availableItems)
        {
            List<ItemSO> items = new();
            foreach (KeyValuePair<ItemSO, SElementToValue<float, int>> item in availableItems)
            {
                ItemSO possibleItem = item.Key;
                float includeInInventoryChance = item.Value.element;
                int maxPossibleItemCount = item.Value.value;

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

        private ActiveAbilitySO[] ChooseEnemyActiveAbilities(FighterConfigurationSO fighterConfiguration)
        {
            ActiveAbilitySO[] availableActiveAbilities = Array.ConvertAll(
                fighterConfiguration.AvailableActiveAbilities.ToArray(),
                activeAbility => activeAbility as ActiveAbilitySO
            );
            return Randomizer.GetRandomUniqueElementsFromArray(
                availableActiveAbilities,
                fighterConfiguration.ActiveAbilitiesCapacity
            );
        }

        private PassiveAbilitySO[] ChooseEnemyPassiveAbilities(FighterConfigurationSO fighterConfiguration)
        {
            PassiveAbilitySO[] availablePassiveAbilities = Array.ConvertAll(
                fighterConfiguration.AvailablePassiveAbilities.ToArray(),
                passiveAbility => passiveAbility as PassiveAbilitySO
            );
            return Randomizer.GetRandomUniqueElementsFromArray(
                availablePassiveAbilities,
                fighterConfiguration.PassiveAbilitiesCapacity
            );
        }

        private KeyValuePair<string, EntityConfigurationSO>[] BuildDevFighterConfMapping(
            EntityConfigurationSO[] devEntityConfs)
        {
            return devEntityConfs.Select(
                devEntityConf => new KeyValuePair<string, EntityConfigurationSO>(null, devEntityConf)
            ).ToArray();
        }
    }
}