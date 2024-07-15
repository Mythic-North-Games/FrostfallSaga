using System;
using System.Collections;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
	public class KingdomToFightTransitioner : MonoBehaviour
	{
		[SerializeField] private EntitiesGroupsManager _entitiesGroupsManager;
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
        /// <param name="enemiesGroup">The encountered enemies group.</param>
        /// <param name="heroGroupInitiating">True if the hero group is initiating the fight, false otherwise.</param>
		private void OnEnemiesGroupEncountered(EntitiesGroup heroGroup, EnemiesGroup enemiesGroup, bool heroGroupInitiating)
		{
			StartCoroutine(StartEncounterAnimation(heroGroup, enemiesGroup, heroGroupInitiating));
		}

        /// <summary>
        /// Once the group has moved, some other animation can be done, but for now, end it and start the fight.
        /// </summary>
        private void OnInitiatorGroupMoved(EntitiesGroup groupThatMoved, Cell destinationCell)
        {
            groupThatMoved.OnEntityGroupMoved -= OnInitiatorGroupMoved;
            _onEncounterAnimationEnded?.Invoke();
        }

        /// <summary>
		/// Plays a ready to fight animation then make the initiating group move to the targeted group.
		/// </summary>
		private IEnumerator StartEncounterAnimation(EntitiesGroup heroGroup, EnemiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            Entity heroEntity = heroGroup.GetDisplayedEntity();
            Entity enemyEntity = enemiesGroup.GetDisplayedEntity();

            // Make groups rotate to watch each other
            heroEntity.EntityVisualMovementController.RotateTowardsCell(enemiesGroup.Cell);
            enemyEntity.EntityVisualMovementController.RotateTowardsCell(heroGroup.Cell);

            // Play ready to fight animation for a while
            heroEntity.EntityAnimationController.PlayAnimationState("ReadyToFight");
            enemyEntity.EntityAnimationController.PlayAnimationState("ReadyToFight");
            yield return new WaitForSeconds(_readyToFightAnimationDuration);

            // Make initiator group go to the cell of its enemy
            EntitiesGroup initiatorGroup = heroGroupInitiating ? heroGroup : enemiesGroup;
            initiatorGroup.OnEntityGroupMoved += OnInitiatorGroupMoved;
            initiatorGroup.MoveToCell(enemiesGroup.Cell, true);
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
            _kingdomData.hexGrid = _entitiesGroupsManager.KingdomGrid;
            _kingdomData.heroGroup = _entitiesGroupsManager.HeroGroup;
            _kingdomData.enemiesGroups = _entitiesGroupsManager.EnemiesGroups;
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

			_entitiesGroupsManager.OnEnemiesGroupEncountered += OnEnemiesGroupEncountered;
            _onEncounterAnimationEnded += OnEncounterAnimationEnded;
		}

        private void OnDisable()
		{
            if (_entitiesGroupsManager == null)
            {
                Debug.LogWarning("No entities groups manager found. Can't tear down properly.");
                return;
            }

			_entitiesGroupsManager.OnEnemiesGroupEncountered -= OnEnemiesGroupEncountered;
            _onEncounterAnimationEnded -= OnEncounterAnimationEnded;
		}
        #endregion
	}
}