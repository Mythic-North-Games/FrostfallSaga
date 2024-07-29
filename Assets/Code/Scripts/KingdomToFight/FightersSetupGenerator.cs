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

        private void OnEnemiesGroupEncountered(EntitiesGroup heroGroup, EnemiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            List<EntityConfigurationSO> allies = new();
            heroGroup.Entities.ToList().ForEach(entity => allies.Add(entity.EntityConfiguration));

            List<EntityConfigurationSO> enemies = new();
            enemiesGroup.Entities.ToList().ForEach(entity => enemies.Add(entity.EntityConfiguration));

            GenerateAndSaveFightersForFight(allies.ToArray(), enemies.ToArray());
        }

        private void GenerateAndSaveFightersForFight(EntityConfigurationSO[] allies, EntityConfigurationSO[] enemies)
        {
            if (_preFightData == null)
            {
                Debug.LogError("No PreFightData scriptable object assigned to fighters generator.");
                return;
            }

            List<FighterSetup> alliesFighterSetup = new();
            foreach (EntityConfigurationSO allyEntity in allies)
            {
                PersistedFighterConfigurationSO allyFighterConfiguration = (PersistedFighterConfigurationSO)_entityToFighterDB.DB.First(
                    entityToFighter => entityToFighter.entityID == allyEntity.EntityID
                ).fighterConfiguration;
                alliesFighterSetup.Add(GenerateFighterSetupFromPersistingConfiguration(allyFighterConfiguration));
            }

            List<FighterSetup> enemiesFighterSetup = new();
            foreach (EntityConfigurationSO enemyEntity in enemies)
            {
                FighterConfigurationSO enemyFighterConfiguration = _entityToFighterDB.DB.First(
                    entityToFighter => entityToFighter.entityID == enemyEntity.EntityID
                ).fighterConfiguration;
                enemiesFighterSetup.Add(GenerateFighterSetupFromNonPersistingConfiguration(enemyFighterConfiguration));
            }

            _preFightData.alliesFighterSetup = alliesFighterSetup.ToArray();
            _preFightData.enemiesFighterSetup = enemiesFighterSetup.ToArray();
        }

        private FighterSetup GenerateFighterSetupFromNonPersistingConfiguration(FighterConfigurationSO fighterConfiguration)
        {
            return new(
                fighterConfiguration.name,
                fighterConfiguration.ExtractFighterStats(),
                fighterConfiguration.DirectAttackTargeter,
                fighterConfiguration.DirectAttackActionPointsCost,
                fighterConfiguration.DirectAttackEffects,
                fighterConfiguration.DirectAttackAnimation,
                GetRandomActiveAbilities(
                    fighterConfiguration.AvailableActiveAbilities,
                    fighterConfiguration.ActiveAbilitiesCapacity
                ),
                fighterConfiguration.ReceiveDamageAnimationStateName,
                fighterConfiguration.HealSelfAnimationStateName
            );
        }

        private FighterSetup GenerateFighterSetupFromPersistingConfiguration(PersistedFighterConfigurationSO fighterConfiguration)
        {
            return new(
                fighterConfiguration.name,
                fighterConfiguration.ExtractFighterStats(),
                fighterConfiguration.DirectAttackTargeter,
                fighterConfiguration.DirectAttackActionPointsCost,
                fighterConfiguration.DirectAttackEffects,
                fighterConfiguration.DirectAttackAnimation,
                fighterConfiguration.EquipedActiveAbilities,
                fighterConfiguration.ReceiveDamageAnimationStateName,
                fighterConfiguration.HealSelfAnimationStateName
            );
        }

        #region Setup and teardown
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
                Debug.LogWarning("No entities groups manager found. Can't teardown properly.");
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