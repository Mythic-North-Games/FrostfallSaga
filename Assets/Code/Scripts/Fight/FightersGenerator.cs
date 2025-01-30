using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.InventorySystem;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;

namespace FrostfallSaga.Fight
{
    public class FightersGenerator : MonoBehaviour
    {
        public Action<Fighter[], Fighter[]> onFightersGenerated;

        [SerializeField] private WorldGameObjectInstantiator _worldGameObjectInstantiator;
        [SerializeField] private EntityConfigurationSO[] _devAlliesConfs;
        [SerializeField] private EntityConfigurationSO[] _devEnemiesConfs;

        private void Start()
        {
            PreFightData preFightData = GameStateManager.Instance.GetPreFightData();

            // Adjust fighter to build based on pre fight data or dev configuration
            KeyValuePair<string, EntityConfigurationSO>[] alliesFighterConf = (
                preFightData.alliesEntityConf != null && preFightData.alliesEntityConf.Length > 0 ?
                preFightData.alliesEntityConf :
                BuildDevFighterConfMapping(_devAlliesConfs)
            );
            KeyValuePair<string, EntityConfigurationSO>[] enemiesFighterConf = (
                preFightData.enemiesEntityConf != null && preFightData.enemiesEntityConf.Length > 0 ?
                preFightData.enemiesEntityConf :
                BuildDevFighterConfMapping(_devEnemiesConfs)
            );

            Debug.Log("Start generating fighters...");
            List<Fighter> allies = new();
            alliesFighterConf.ToList().ForEach(allyFighterConf =>
                allies.Add(SpawnAndSetupFighter(allyFighterConf.Value, allyFighterConf.Key))
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

            onFightersGenerated?.Invoke(allies.ToArray(), enemies.ToArray());
            Debug.Log("Fighters generated.");

            if (alliesFighterConf == preFightData.alliesEntityConf)
            {
                GameStateManager.Instance.CleanPreFightData();
                return;
            }

            Debug.Log("Fight launched in dev mode. Not went through kingdom first.");
        }

        private Fighter SpawnAndSetupFighter(
            EntityConfigurationSO entityConfiguration,
            string sessionId = null,
            string nameSuffix = ""
        )
        {
            // Spawn fighter game object
            FighterConfigurationSO fighterConfiguration = entityConfiguration.FighterConfiguration;
            GameObject fighterGameObject = _worldGameObjectInstantiator.Instantiate(fighterConfiguration.FighterPrefab);
            fighterGameObject.name = new($"{entityConfiguration.Name}{nameSuffix}");

            // Setup spawned fighter
            Fighter fighter = fighterGameObject.GetComponent<Fighter>();
            if (entityConfiguration.FighterConfiguration is PersistedFighterConfigurationSO)
            {
                SetupAllyFighter(fighter, entityConfiguration, sessionId);
            }
            else
            {
                SetupEnemyFighter(fighter, entityConfiguration, sessionId);
            }

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
                inventory: GenerateEnemyFightInventory(),
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
            PersistedFighterConfigurationSO fighterConfiguration = entityConfiguration.FighterConfiguration as PersistedFighterConfigurationSO;
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
                equippedActiveAbilities: activeAbilities,
                equippedPassiveAbilities: passiveAbilities,
                inventory: fighterConfiguration.Inventory,
                sessionId: sessionId
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

        private KeyValuePair<string, EntityConfigurationSO>[] BuildDevFighterConfMapping(EntityConfigurationSO[] devEntityConfs)
        {
            return devEntityConfs.Select(
                devEntityConf => new KeyValuePair<string, EntityConfigurationSO>(null, devEntityConf)
            ).ToArray();
        }

        #region Setup & Teardown
        private void Awake()
        {
            if (_worldGameObjectInstantiator == null)
            {
                _worldGameObjectInstantiator = FindObjectOfType<WorldGameObjectInstantiator>();
            }
            if (_worldGameObjectInstantiator == null)
            {
                Debug.LogError("No world game object instantiator found. Can't spawn objects.");
                return;
            }
        }
        #endregion
    }
}