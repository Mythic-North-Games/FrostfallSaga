using System;
using System.Collections;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using System.Collections.Generic;

namespace FrostfallSaga.Kingdom
{
    public class KingdomToFightTransitioner : MonoBehaviour
    {
        [SerializeField] private EntitiesGroupsManager _entitiesGroupsManager;
        [SerializeField] private EntitiesGroupBuilder _entitiesGroupBuilder;
        [SerializeField] private KingdomDataSO _kingdomData;
        [SerializeField] private SceneTransitioner _sceneTransitioner;
        [SerializeField] private float _readyToFightAnimationDuration = 2f;
        [SerializeField] private float _delayBeforeLoadingSceneAfterReadyAnimation = 2f;
        [SerializeField] private string _fightSceneName;

        private Action _onEncounterAnimationEnded;

        /// <summary>
        /// Start the encounter animation before saving the kingdom state and launching the fight scene.
        /// </summary>
        /// <param name="heroGroup">The hero group.</param>
        /// <param name="EntitiesGroup">The encountered enemies group.</param>
        /// <param name="heroGroupInitiating">True if the hero group is initiating the fight, false otherwise.</param>
		private void OnEnemiesGroupEncountered(EntitiesGroup heroGroup, EntitiesGroup EntitiesGroup, bool heroGroupInitiating)
        {
            StartCoroutine(StartEncounterAnimation(heroGroup, EntitiesGroup, heroGroupInitiating));
        }

        /// <summary>
        /// Once the group has moved, some other animation can be done, but for now, end it and start the fight.
        /// </summary>
        private void OnInitiatorGroupMoved(EntitiesGroup groupThatMoved, Cell destinationCell)
        {
            groupThatMoved.onEntityGroupMoved -= OnInitiatorGroupMoved;
            _onEncounterAnimationEnded?.Invoke();
        }

        /// <summary>
		/// Plays a ready to fight animation then make the initiating group move to the targeted group.
		/// </summary>
		private IEnumerator StartEncounterAnimation(EntitiesGroup heroGroup, EntitiesGroup EntitiesGroup, bool heroGroupInitiating)
        {
            Entity heroEntity = heroGroup.GetDisplayedEntity();
            Entity enemyEntity = EntitiesGroup.GetDisplayedEntity();

            // Make groups rotate to watch each other
            heroEntity.EntityVisualMovementController.RotateTowardsCell(EntitiesGroup.cell);
            enemyEntity.EntityVisualMovementController.RotateTowardsCell(heroGroup.cell);

            // Play ready to fight animation for a while
            heroEntity.EntityAnimationController.PlayAnimationState("ReadyToFight");
            enemyEntity.EntityAnimationController.PlayAnimationState("ReadyToFight");
            yield return new WaitForSeconds(_readyToFightAnimationDuration);

            // Make initiator group go to the cell of its enemy
            EntitiesGroup initiatorGroup = heroGroupInitiating ? heroGroup : EntitiesGroup;
            initiatorGroup.onEntityGroupMoved += OnInitiatorGroupMoved;
            initiatorGroup.MoveToCell(EntitiesGroup.cell, true);
        }

        private void OnEncounterAnimationEnded()
        {
            SaveKingdomData();
            StartCoroutine(StartFightScene());
        }

        private IEnumerator StartFightScene()
        {
            yield return new WaitForSeconds(_delayBeforeLoadingSceneAfterReadyAnimation);
            _sceneTransitioner.FadeInToScene(_fightSceneName);
        }

        private void SaveKingdomData()
        {
            _kingdomData.heroGroupData = _entitiesGroupBuilder.ExtractEntitiesGroupDataFromEntiesGroup(_entitiesGroupsManager.HeroGroup);

            List<EntitiesGroupData> enemiesGroupsData = new();
            _entitiesGroupsManager.EnemiesGroups.ForEach(group =>
            {
                enemiesGroupsData.Add(_entitiesGroupBuilder.ExtractEntitiesGroupDataFromEntiesGroup(group));
            });
            _kingdomData.enemiesGroupsData = enemiesGroupsData.ToArray();

            Debug.Log("KingdomConfiguration Saved !");
        }

        #region Setup and teardown
        private void OnEnable()
        {
            if (_entitiesGroupsManager == null)
            {
                _entitiesGroupsManager = FindObjectOfType<EntitiesGroupsManager>();
            }
            if (_entitiesGroupsManager == null)
            {
                Debug.LogError("No entities groups manager found. Can't transition to fight scene.");
                return;
            }

            if (_kingdomData == null)
            {
                Debug.LogError("No KingdomData scriptable object given. Actual kingdom state will be lost after fight.");
                return;
            }

            _entitiesGroupsManager.onEnemiesGroupEncountered += OnEnemiesGroupEncountered;
            _onEncounterAnimationEnded += OnEncounterAnimationEnded;
        }

        private void OnDisable()
        {
            if (_entitiesGroupsManager == null)
            {
                Debug.LogWarning("No entities groups manager found. Can't tear down properly.");
                return;
            }

            _entitiesGroupsManager.onEnemiesGroupEncountered -= OnEnemiesGroupEncountered;
            _onEncounterAnimationEnded -= OnEncounterAnimationEnded;
        }
        #endregion
    }
}