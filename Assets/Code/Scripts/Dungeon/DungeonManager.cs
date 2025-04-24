using System;
using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Dungeon;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Dungeon
{
    public class DungeonManager : MonoBehaviour
    {
        public Action onDungeonCompleted;
        private GameStateManager _gameStateManager;

        private void Awake()
        {
            HeroTeamQuests.Instance.InitializeQuests(this);

            _gameStateManager = GameStateManager.Instance;
            DungeonState dungeonState = _gameStateManager.GetDungeonState();

            DungeonConfigurationSO dungeonConfiguration = dungeonState.DungeonConfiguration;
            if (!dungeonConfiguration)
            {
                Debug.LogError("No dungeon configuration found. Not able to generate dungeon fights.");
                return;
            }

            if (!dungeonState.AlliesWonLastFight || dungeonState.IsDungeonCompleted())
            {
                if(dungeonState.IsDungeonCompleted()) onDungeonCompleted?.Invoke();

                Debug.Log(
                    "Dungeon completed or allies lost last fight. Cleaning dungeon state and transitioning to kingdom..."
                );
                dungeonState.DungeonConfiguration.CompleteDungeon();
                _gameStateManager.CleanDungeonState();
                SceneTransitioner.BrutalSceneTransition(EScenesName.KINGDOM.ToSceneString());
                return;
            }

            if (dungeonState.IsAtBossFight())
            {
                Debug.Log("Preparing final boss fight...");
                PrepareDungeonFight(dungeonConfiguration.BossFightConfiguration);
            }
            else
            {
                Debug.Log($"Preparing dungeon fight {dungeonState.CurrentDungeonFightIndex + 1}...");
                PrepareDungeonFight(
                    dungeonConfiguration.PreBossFightConfigurations[dungeonState.CurrentDungeonFightIndex]);
            }

            Debug.Log("Dungeon fight prepared. Launching fight scene...");
            SceneTransitioner.BrutalSceneTransition(EScenesName.FIGHT.ToSceneString());
        }

        private void PrepareDungeonFight(DungeonFightConfiguration dungeonFightConfiguration)
        {
            _gameStateManager.SavePreFightData(
                HeroTeam.Instance.GetAliveHeroesEntityConfig(),
                GetDungeonFightEnemies(dungeonFightConfiguration),
                EFightOrigin.DUNGEON,
                null
            );
        }

        private static KeyValuePair<string, EntityConfigurationSO>[] GetDungeonFightEnemies(
            DungeonFightConfiguration dungeonFightConfiguration)
        {
            // Randomize the number of optional enemies
            int optionalEnemiesCount = Randomizer.GetRandomIntBetween(
                dungeonFightConfiguration.MinOptionalEnemies,
                dungeonFightConfiguration.MaxOptionalEnemies
            );
            int totalEnemiesCount = dungeonFightConfiguration.MandatoryEnemies.Length + optionalEnemiesCount;
            KeyValuePair<string, EntityConfigurationSO>[] enemies =
                new KeyValuePair<string, EntityConfigurationSO>[totalEnemiesCount];

            // Add mandatory enemies
            for (int i = 0; i < dungeonFightConfiguration.MandatoryEnemies.Length; i++)
                enemies[i] =
                    new KeyValuePair<string, EntityConfigurationSO>(null,
                        dungeonFightConfiguration.MandatoryEnemies[i]);

            // Add random optional enemies
            for (int i = dungeonFightConfiguration.MandatoryEnemies.Length; i < totalEnemiesCount; i++)
                enemies[i] = new KeyValuePair<string, EntityConfigurationSO>(null,
                    Randomizer.GetRandomElementFromArray(dungeonFightConfiguration.OptionalEnemies));

            return enemies;
        }
    }
}