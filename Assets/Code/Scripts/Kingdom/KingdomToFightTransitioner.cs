using System;
using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomToFightTransitioner : MonoBehaviour
    {
        [SerializeField] private KingdomManager kingdomManager;
        [SerializeField] private SceneTransitioner sceneTransitioner;
        [SerializeField] private float readyToFightAnimationDuration = 2f;
        [SerializeField] private float delayBeforeLoadingSceneAfterReadyAnimation = 10f;
        private Action _onEncounterAnimationEnded;

        #region Setup and tear down

        private void Awake()
        {
            kingdomManager ??= FindObjectOfType<KingdomManager>();

            if (kingdomManager == null)
            {
                Debug.LogError("No KingdomManager found. Can't transition to fight scene.");
                return;
            }

            sceneTransitioner ??= FindObjectOfType<SceneTransitioner>();

            if (!sceneTransitioner)
            {
                Debug.LogError("No SceneTransitioner found. Can't transition to fight scene.");
                return;
            }

            kingdomManager.OnEnemiesGroupEncountered += OnEnemiesGroupEncountered;
            _onEncounterAnimationEnded += OnEncounterAnimationEnded;
        }

        #endregion

        /// <summary>
        ///     Start the encounter animation before saving the kingdom state and launching the fight scene.
        /// </summary>
        /// <param name="enemiesGroup">The encountered enemies group.</param>
        /// <param name="heroGroupInitiating">True if the hero group is initiating the fight, false otherwise.</param>
        private void OnEnemiesGroupEncountered(EntitiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            PrepareAndSavePreFightData(enemiesGroup);
            StartCoroutine(StartEncounterAnimation(enemiesGroup, heroGroupInitiating));
        }

        /// <summary>
        ///     Once the group has moved, some other animation can be done, but for now, end it and start the fight.
        /// </summary>
        private void OnInitiatorGroupMoved(EntitiesGroup groupThatMoved, Cell destinationCell)
        {
            groupThatMoved.OnEntityGroupMoved -= OnInitiatorGroupMoved;
            _onEncounterAnimationEnded?.Invoke();
        }

        /// <summary>
        ///     Plays a ready to fight animation then make the initiating group move to the targeted group.
        /// </summary>
        private IEnumerator StartEncounterAnimation(EntitiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            EntitiesGroup heroGroup = kingdomManager.HeroGroup;
            Entity heroEntity = heroGroup.GetDisplayedEntity();
            Entity enemyEntity = enemiesGroup.GetDisplayedEntity();

            // Make groups rotate to watch each other
            heroEntity.MovementController.RotateTowardsCell(enemiesGroup.cell);
            enemyEntity.MovementController.RotateTowardsCell(heroGroup.cell);

            // Play ready to fight animation for a while
            heroEntity.AnimationController.PlayAnimationState("ReadyToFight");
            enemyEntity.AnimationController.PlayAnimationState("ReadyToFight");
            yield return new WaitForSeconds(readyToFightAnimationDuration);

            // Make initiator group go to the cell of its enemy
            EntitiesGroup initiatorGroup = heroGroupInitiating ? heroGroup : enemiesGroup;
            EntitiesGroup attackedGroup = heroGroupInitiating ? enemiesGroup : heroGroup;
            initiatorGroup.OnEntityGroupMoved += OnInitiatorGroupMoved;
            initiatorGroup.MoveToCell(attackedGroup.cell, true);
        }

        private void OnEncounterAnimationEnded()
        {
            kingdomManager.SaveKingdomState();
            StartCoroutine(StartFightScene());
        }

        private IEnumerator StartFightScene()
        {
            yield return new WaitForSeconds(delayBeforeLoadingSceneAfterReadyAnimation);
            Debug.Log("Transitioning to fight");
            sceneTransitioner.FadeInToScene(EScenesName.FIGHT.ToSceneString());
        }

        private void PrepareAndSavePreFightData(EntitiesGroup enemiesGroup)
        {
            KeyValuePair<string, EntityConfigurationSO>[] enemiesFighterConfigs =
                new KeyValuePair<string, EntityConfigurationSO>[enemiesGroup.Entities.Length];
            for (int i = 0; i < enemiesGroup.Entities.Length; i++)
            {
                Entity enemyGroupEntity = enemiesGroup.Entities[i];
                enemiesFighterConfigs[i] = new KeyValuePair<string, EntityConfigurationSO>(enemyGroupEntity.SessionId,
                    enemyGroupEntity.EntityConfiguration);
            }

            CellAnalysis.AnalyzeAtCell(enemiesGroup.cell, kingdomManager.KingdomGrid);
            CellAnalysis.PrintAnalysisDict(); //TEMP à retirer après le Debug

            GameStateManager.Instance.SavePreFightData(
                HeroTeam.Instance.GetAliveHeroesEntityConfig(),
                enemiesFighterConfigs,
                EFightOrigin.KINGDOM,
                CellAnalysis.CELLS_BY_DIRECTION
            );
        }
    }
}