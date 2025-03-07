using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.InventorySystem;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightersGenerator
    {
        private readonly EntityConfigurationSO[] _devAlliesConfs;
        private readonly EntityConfigurationSO[] _devEnemiesConfs;

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
                ChooseEnemyActiveAbilities(fighterConfiguration),
                ChooseEnemyPassiveAbilities(fighterConfiguration),
                GenerateEnemyFightInventory(),
                sessionId
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

        private Inventory GenerateEnemyFightInventory()
        {
            Inventory inventory = new();
            inventory.AddItem(Resources.Load<WeaponSO>(Inventory.DefaultWeaponResourcePath));
            return inventory;
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