using System.Collections;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.Kingdom
{
	public class KingdomToFightTransitioner : MonoBehaviour
	{
		[SerializeField] private KingdomManager _kingdomManager;
		[SerializeField] private SceneTransitioner _sceneTransitioner;
		[SerializeField] private float _readyToFightAnimationDuration = 2f;
		[SerializeField] private float _delayBeforeLoadingSceneAfterReadyAnimation = 2f;
		[SerializeField] private string _fightSceneName;

		public void OnEnable()
		{
			_kingdomManager.OnEnemiesGroupEncountered += OnEnemiesGroupEncountered;
		}

		private void OnEnemiesGroupEncountered(EntitiesGroup heroGroup, EnemiesGroup enemiesGroup, bool heroGroupInitiating)
		{
			StartCoroutine(PlayReadyAnimationThenLoadScene(heroGroup, enemiesGroup, heroGroupInitiating));
		}

		/// <summary>
		/// Plays a ready to fight animation then make the initiating group move to the targeted group then load fight scene.
		/// </summary>
		private IEnumerator PlayReadyAnimationThenLoadScene(EntitiesGroup heroGroup, EnemiesGroup enemiesGroup, bool heroGroupInitiating)
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
			if (heroGroupInitiating)
			{
				heroGroup.MoveToCell(enemiesGroup.Cell, true);
			}
			else
			{
				enemiesGroup.MoveToCell(heroGroup.Cell, true);
			}
			yield return new WaitForSeconds(_delayBeforeLoadingSceneAfterReadyAnimation);

			// Start the fight scene
			_sceneTransitioner.FadeInToScene(_fightSceneName);
		}

		private void OnDisable()
		{
			_kingdomManager.OnEnemiesGroupEncountered -= OnEnemiesGroupEncountered;
		}
	}
}