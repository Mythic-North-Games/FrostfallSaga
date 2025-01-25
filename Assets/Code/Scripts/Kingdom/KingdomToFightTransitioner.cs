using System;
using System.Collections;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.Kingdom
{
    public class KingdomToFightTransitioner : MonoBehaviour
    {
        [SerializeField] private EntitiesGroupsManager _entitiesGroupsManager;
        [SerializeField] private SceneTransitioner _sceneTransitioner;
        [SerializeField] private float _readyToFightAnimationDuration = 2f;
        [SerializeField] private float _delayBeforeLoadingSceneAfterReadyAnimation = 10f;
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
		private IEnumerator StartEncounterAnimation(EntitiesGroup heroGroup, EntitiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            Entity heroEntity = heroGroup.GetDisplayedEntity();
            Entity enemyEntity = enemiesGroup.GetDisplayedEntity();

            // Make groups rotate to watch each other
            heroEntity.MovementController.RotateTowardsCell(enemiesGroup.cell);
            enemyEntity.MovementController.RotateTowardsCell(heroGroup.cell);

            // Play ready to fight animation for a while
            heroEntity.AnimationController.PlayAnimationState("ReadyToFight");
            enemyEntity.AnimationController.PlayAnimationState("ReadyToFight");
            yield return new WaitForSeconds(_readyToFightAnimationDuration);

            // Make initiator group go to the cell of its enemy
            EntitiesGroup initiatorGroup = heroGroupInitiating ? heroGroup : enemiesGroup;
            EntitiesGroup attackedGroup = heroGroupInitiating ? enemiesGroup : heroGroup;
            initiatorGroup.onEntityGroupMoved += OnInitiatorGroupMoved;
            initiatorGroup.MoveToCell(attackedGroup.cell, true);
        }

        private void OnEncounterAnimationEnded()
        {
            KingdomState.Instance.SaveKingdomData(
                _entitiesGroupsManager.HeroGroup,
                _entitiesGroupsManager.EnemiesGroups,
                _entitiesGroupsManager.CityBuildings
            );
            StartCoroutine(StartFightScene());
        }

        private IEnumerator StartFightScene()
        {
            yield return new WaitForSeconds(_delayBeforeLoadingSceneAfterReadyAnimation);
            Debug.Log("Transitioning to fight");
            _sceneTransitioner.FadeInToScene(_fightSceneName);
        }

        #region Setup and tear down
        private void Awake()
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
            _entitiesGroupsManager.onEnemiesGroupEncountered += OnEnemiesGroupEncountered;
            _onEncounterAnimationEnded += OnEncounterAnimationEnded;
        }
        #endregion
    }
}