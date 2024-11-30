using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Fight;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.GameItems;

namespace FrostfallSaga.KingdomToFight
{
    public class FightersGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject _fighterPrefab;
        [SerializeField] private EntityToFighterDBSO _entityToFighterDB;
        [SerializeField] private PreFightDataSO _preFightData;
        [SerializeField] private EntitiesGroupsManager _entitiesGroupsManager;

        private void OnEnemiesGroupEncountered(EntitiesGroup heroGroup, EntitiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            GenerateAndSaveFightersForFight(heroGroup.Entities, enemiesGroup.Entities);
        }

        private void GenerateAndSaveFightersForFight(Entity[] heroGroupEntities, Entity[] enemiesGroupEntities)
        {
            if (_preFightData == null)
            {
                Debug.LogError("No PreFightData scriptable object assigned to fighters generator.");
                return;
            }

            List<FighterSetup> alliesFighterSetup = new();
            foreach (Entity allyEntity in heroGroupEntities)
            {
                PersistedFighterConfigurationSO allyFighterConfiguration = (PersistedFighterConfigurationSO)_entityToFighterDB.DB.First(
                    entityToFighter => entityToFighter.entityID == allyEntity.EntityConfiguration.EntityID
                ).fighterConfiguration;
                alliesFighterSetup.Add(
                    GenerateFighterSetupFromPersistingConfiguration(
                        allyEntity.EntityConfiguration,
                        allyFighterConfiguration,
                        allyEntity.sessionId
                    )
                );
            }

            List<FighterSetup> enemiesFighterSetup = new();
            foreach (Entity enemyEntity in enemiesGroupEntities)
            {
                FighterConfigurationSO enemyFighterConfiguration = _entityToFighterDB.DB.First(
                    entityToFighter => entityToFighter.entityID == enemyEntity.EntityConfiguration.EntityID
                ).fighterConfiguration;
                enemiesFighterSetup.Add(
                    GenerateFighterSetupFromNonPersistingConfiguration(
                        enemyEntity.EntityConfiguration,
                        enemyFighterConfiguration,
                        enemyEntity.sessionId
                    )
                );
            }

            _preFightData.alliesFighterSetup = alliesFighterSetup.ToArray();
            _preFightData.enemiesFighterSetup = enemiesFighterSetup.ToArray();
        }

        private FighterSetup GenerateFighterSetupFromNonPersistingConfiguration(
            EntityConfigurationSO entityConfiguration,
            FighterConfigurationSO fighterConfiguration,
            string entitySessionId
        )
        {
            Inventory defaultInventory = new();
            defaultInventory.AddItem(Resources.Load<WeaponSO>(Inventory.DefaultWeaponResourcePath));

            return new(
                fighterConfiguration.name,
                entitySessionId,
                entityConfiguration.EntityID,
                entityConfiguration.Icon,
                entityConfiguration.DiamondIcon,
                fighterConfiguration.ExtractFighterStats(),
                fighterConfiguration.FighterClass,
                fighterConfiguration.PersonalityTrait,
                defaultInventory,
                fighterConfiguration.DirectAttackAnimation,
                GetRandomActiveAbilities(
                    fighterConfiguration.AvailableActiveAbilities,
                    fighterConfiguration.ActiveAbilitiesCapacity
                ),
                GetRandomPassiveAbilities(
                    fighterConfiguration.AvailablePassiveAbilities,
                    fighterConfiguration.PassiveAbilitiesCapacity
                ),
                fighterConfiguration.ReceiveDamageAnimationName,
                fighterConfiguration.HealSelfAnimationName,
                fighterConfiguration.ReduceStatAnimationName,
                fighterConfiguration.IncreaseStatAnimationName
            );
        }

        private FighterSetup GenerateFighterSetupFromPersistingConfiguration(
            EntityConfigurationSO entityConfiguration,
            PersistedFighterConfigurationSO persistedFighterConfiguration,
            string entitySessionId
        )
        {
            return new(
                persistedFighterConfiguration.name,
                entitySessionId,
                entityConfiguration.EntityID,
                entityConfiguration.Icon,
                entityConfiguration.DiamondIcon,
                persistedFighterConfiguration.ExtractFighterStats(),
                persistedFighterConfiguration.FighterClass,
                persistedFighterConfiguration.PersonalityTrait,
                persistedFighterConfiguration.Inventory,
                persistedFighterConfiguration.DirectAttackAnimation,
                persistedFighterConfiguration.EquipedActiveAbilities,
                persistedFighterConfiguration.EquipedPassiveAbilities,
                persistedFighterConfiguration.ReceiveDamageAnimationName,
                persistedFighterConfiguration.HealSelfAnimationName,
                persistedFighterConfiguration.ReduceStatAnimationName,
                persistedFighterConfiguration.IncreaseStatAnimationName
            );
        }

        #region Setup and tear down
        private void OnEnable()
        {
            if (_preFightData == null)
            {
                Debug.LogError("No PreFightData scriptable object assigned to fighters generator.");
                return;
            }
            if (_entityToFighterDB == null)
            {
                Debug.LogError("No EntitiyToFighterDB scriptable object assigned to fighters generator.");
                return;
            }

            if (_entitiesGroupsManager == null)
            {
                _entitiesGroupsManager = FindObjectOfType<EntitiesGroupsManager>();
            }
            if (_entitiesGroupsManager == null)
            {
                Debug.LogError("No entities groups manager found.");
                return;
            }

            _entitiesGroupsManager.onEnemiesGroupEncountered += OnEnemiesGroupEncountered;
        }

        private void OnDisable()
        {
            if (_entitiesGroupsManager == null)
            {
                _entitiesGroupsManager = FindObjectOfType<EntitiesGroupsManager>();
            }
            if (_entitiesGroupsManager == null)
            {
                Debug.LogWarning("No entities groups manager found. Can't tear down properly.");
                return;
            }

            _entitiesGroupsManager.onEnemiesGroupEncountered -= OnEnemiesGroupEncountered;
        }
        #endregion

        private static ActiveAbilitySO[] GetRandomActiveAbilities(ActiveAbilitySO[] activeAbalities, int count)
        {
            List<ActiveAbilitySO> availableActiveAbilities = new(activeAbalities);
            List<ActiveAbilitySO> equipedActiveAbilities = new();
            while (equipedActiveAbilities.Count < count && availableActiveAbilities.Count > 0)
            {
                int randomActiveAbilityIndex = Randomizer.GetRandomIntBetween(0, availableActiveAbilities.Count);
                equipedActiveAbilities.Add(availableActiveAbilities[randomActiveAbilityIndex]);
                availableActiveAbilities.RemoveAt(randomActiveAbilityIndex);
            }
            return equipedActiveAbilities.ToArray();
        }

        private static PassiveAbilitySO[] GetRandomPassiveAbilities(PassiveAbilitySO[] availablePassiveAbilities, int passiveAbilitiesCapacity)
        {
            int nbPassiveAbilitiesToAdd = Randomizer.GetRandomIntBetween(0, passiveAbilitiesCapacity);
            List<PassiveAbilitySO> equipedPassiveAbilities = new();
            while (equipedPassiveAbilities.Count < nbPassiveAbilitiesToAdd && availablePassiveAbilities.Length > 0)
            {
                int randomPassiveAbilityIndex = Randomizer.GetRandomIntBetween(0, availablePassiveAbilities.Length);
                equipedPassiveAbilities.Add(availablePassiveAbilities[randomPassiveAbilityIndex]);
                availablePassiveAbilities = availablePassiveAbilities.Where((_, index) => index != randomPassiveAbilityIndex).ToArray();
            }
            return equipedPassiveAbilities.ToArray();
        }
    }
}