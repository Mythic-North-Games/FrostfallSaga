using System.Collections;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
	public class KingdomToFightTransitioner : MonoBehaviour
	{
		[SerializeField] private KingdomManager _kingdomManager;
		[SerializeField] private SceneTransitioner _sceneTransitioner;
		[SerializeField] private float _readyToFightAnimationDuration = 2f;
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
			heroGroup.GetDisplayedEntity().EntityAnimationController.PlayAnimationState("ReadyToFight");
			enemiesGroup.GetDisplayedEntity().EntityAnimationController.PlayAnimationState("ReadyToFight");
			yield return new WaitForSeconds(_readyToFightAnimationDuration);
			if (heroGroupInitiating)
			{
				heroGroup.MoveToCell(enemiesGroup.Cell);
			}
			else
			{
				enemiesGroup.MoveToCell(heroGroup.Cell);
			}
			_sceneTransitioner.FadeInToScene(_fightSceneName);
		}

		private void OnDisable()
		{
			_kingdomManager.OnEnemiesGroupEncountered -= OnEnemiesGroupEncountered;
		}
	}
}