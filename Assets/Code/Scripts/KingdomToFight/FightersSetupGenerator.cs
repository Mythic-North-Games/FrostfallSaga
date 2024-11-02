using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Fight;
using FrostfallSaga.Fight.Fighters;

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
            return new(
                fighterConfiguration.name,
                entitySessionId,
                entityConfiguration.EntityIcon,
                fighterConfiguration.ExtractFighterStats(),
                fighterConfiguration.FighterClass,
                fighterConfiguration.PersonalityTrait,
                fighterConfiguration.DirectAttackTargeter,
                fighterConfiguration.DirectAttackActionPointsCost,
                fighterConfiguration.DirectAttackEffects,
                fighterConfiguration.DirectAttackAnimation,
                GetRandomActiveAbilities(
                    fighterConfiguration.AvailableActiveAbilities,
                    fighterConfiguration.ActiveAbilitiesCapacity
                ),
                fighterConfiguration.ReceiveDamageAnimationName,
                fighterConfiguration.HealSelfAnimationName,
                fighterConfiguration.ReduceStatAnimationName,
                fighterConfiguration.IncreaseStatAnimationName
            );
        }

        private FighterSetup GenerateFighterSetupFromPersistingConfiguration(
            EntityConfigurationSO entityConfiguration,
            PersistedFighterConfigurationSO fighterConfiguration,
            string entitySessionId
        )
        {
            return new(
                fighterConfiguration.name,
                entitySessionId,
                entityConfiguration.EntityIcon,
                fighterConfiguration.ExtractFighterStats(),
                fighterConfiguration.FighterClass,
                fighterConfiguration.PersonalityTrait,
                fighterConfiguration.DirectAttackTargeter,
                fighterConfiguration.DirectAttackActionPointsCost,
                fighterConfiguration.DirectAttackEffects,
                fighterConfiguration.DirectAttackAnimation,
                fighterConfiguration.EquipedActiveAbilities,
                fighterConfiguration.ReceiveDamageAnimationName,
                fighterConfiguration.HealSelfAnimationName,
                fighterConfiguration.ReduceStatAnimationName,
                fighterConfiguration.IncreaseStatAnimationName
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

        private static ActiveAbilityToAnimation[] GetRandomActiveAbilities(ActiveAbilityToAnimation[] activeAbalities, int count)
        {
            List<ActiveAbilityToAnimation> availableActiveAbilities = new(activeAbalities);
            List<ActiveAbilityToAnimation> equipedActiveAbilities = new();
            while (equipedActiveAbilities.Count < count && availableActiveAbilities.Count > 0)
            {
                int randomActiveAbilityIndex = Randomizer.GetRandomIntBetween(0, availableActiveAbilities.Count);
                equipedActiveAbilities.Add(availableActiveAbilities[randomActiveAbilityIndex]);
                availableActiveAbilities.RemoveAt(randomActiveAbilityIndex);
            }
            return equipedActiveAbilities.ToArray();
        }
    }
}