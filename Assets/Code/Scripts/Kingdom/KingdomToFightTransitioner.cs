using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Audio;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Utils.Scenes;
using FrostfallSaga.Utils.Camera;
using UnityEngine;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Kingdom
{
    public class KingdomToFightTransitioner : MonoBehaviour
    {
        [SerializeField] private KingdomManager _kingdomManager;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private float _readyToFightAnimationDuration = 2f;
        [SerializeField] private float _cameraZoomAmount = 10f;
        [SerializeField] private float _cameraZoomDuration = 0.5f;
        [SerializeField] private bool _isPrintAnalysis;

        #region Setup and tear down

        private void Awake()
        {
            _kingdomManager = _kingdomManager != null ? _kingdomManager : FindObjectOfType<KingdomManager>();
            if (!_kingdomManager)
            {
                Debug.LogError("No KingdomManager found. Can't transition to fight scene.");
                return;
            }

            _cameraController = _cameraController != null ? _cameraController : FindObjectOfType<CameraController>();
            if (!_cameraController)
            {
                Debug.LogError("No CameraController found. Can't transition to fight scene.");
                return;
            }

            _kingdomManager.OnEnemiesGroupEncountered += OnEnemiesGroupEncountered;
        }

        #endregion

        /// <summary>
        /// Start the encounter animation before saving the kingdom state and launching the fight scene.
        /// </summary>
        /// <param name="enemiesGroup">The encountered enemies group.</param>
        /// <param name="heroGroupInitiating">True if the hero group is initiating the fight, false otherwise.</param>
        private void OnEnemiesGroupEncountered(EntitiesGroup enemiesGroup, bool heroGroupInitiating)
        {
            PrepareAndSavePreFightData(enemiesGroup);
            StartCoroutine(PlayEncounterAnimationAndLaunchFightScene(enemiesGroup, heroGroupInitiating));
        }

        /// <summary>
        /// Plays a ready to fight animation then make the initiating group move to the targeted group.
        /// </summary>
        private IEnumerator PlayEncounterAnimationAndLaunchFightScene(
            EntitiesGroup enemiesGroup,
            bool heroGroupInitiating
        )
        {
            EntitiesGroup heroGroup = _kingdomManager.HeroGroup;
            EntitiesGroup initiatorGroup = heroGroupInitiating ? heroGroup : enemiesGroup;
            EntitiesGroup attackedGroup = heroGroupInitiating ? enemiesGroup : heroGroup;
            Entity initiatorEntity = initiatorGroup.GetDisplayedEntity();
            Entity attackedEntity = attackedGroup.GetDisplayedEntity();

            // Make initiator group rotate towards the enemy group
            initiatorEntity.MovementController.RotateTowardsCell(enemiesGroup.cell);
            initiatorEntity.AnimationController.PlayAnimationState("ReadyToFight");

            // Make enemy group rotate towards the initiator group only if enemyEntity can see the initiator group
            if (attackedEntity.CanSeeTarget(initiatorEntity.transform))
            {
                attackedEntity.MovementController.RotateTowardsCell(heroGroup.cell);
                attackedEntity.AnimationController.PlayAnimationState("ReadyToFight");
            }
            yield return new WaitForSeconds(_readyToFightAnimationDuration);

            // Make camera zoom in on the initiator group
            _cameraController.ZoomIn(_cameraZoomAmount, _cameraZoomDuration);

            // Make initiator group go to the cell of its enemy
            initiatorEntity.MovementController.Move(
                initiatorGroup.cell,
                attackedGroup.cell,
                true
            );
            attackedGroup.cell.SetOccupier(initiatorGroup);

            // Save the current kingdom state
            _kingdomManager.SaveKingdomState();

            // Start playing the fight scene music
            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlayMusic(audioManager.MusicAudioClips.Fight);

            // Transition to the fight scene
            SceneTransitioner.TransitionToScene(EScenesName.FIGHT.ToSceneString());
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

            Dictionary<HexDirection, Cell> analyze = CellAnalysis.AnalyzeAtCell(
                enemiesGroup.cell,
                _kingdomManager.KingdomGrid,
                _isPrintAnalysis
            );

            GameStateManager.Instance.SavePreFightData(
                HeroTeam.Instance.GetAliveHeroesEntityConfig(),
                enemiesFighterConfigs,
                EFightOrigin.KINGDOM,
                analyze
            );
        }
    }
}