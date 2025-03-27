using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.InterestPoints;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomLoader : MonoBehaviour
    {
        [SerializeField] private KingdomHexGrid kingdomHexGrid;
        [SerializeField] private new CinemachineVirtualCamera camera;
        [SerializeField] public List<AInterestPointConfigurationSO> interestPointConfigs;
        private readonly List<EntitiesGroup> _respawnedEnemiesGroups = new();
        private GameStateManager _gameStateManager;
        private HeroTeam _heroTeam;
        private EntitiesGroup _respawnedHeroGroup;
        public Action OnKingdomLoaded;

        #region Setup & tear down

        private void Awake()
        {
            kingdomHexGrid ??= FindObjectOfType<KingdomHexGrid>();
            if (kingdomHexGrid == null)
            {
                Debug.LogError("No grid found. Can't know where to spawn the entities groups.");
                return;
            }

            _gameStateManager = GameStateManager.Instance;
            _heroTeam = HeroTeam.Instance;
        }

        #endregion

        private void Start()
        {
            kingdomHexGrid.ClearCells();
            kingdomHexGrid.GenerateGrid();
            if (_gameStateManager.IsFirstSceneLaunch())
            {
                Debug.Log("First scene launch. No kingdom to load.");
                FirstSpawnHeroGroup();
                InterestPointBuilder.Instance.FirstBuildInterestPoints(kingdomHexGrid, interestPointConfigs);
                OnKingdomLoaded?.Invoke();
                return;
            }

            Debug.Log("Start loading kingdom.");
            DestroyDevOccupiers();
            LoadKingdomAsBeforeFight();
            RestoreHeroGroup();

            if (!_gameStateManager.HasFightJustOccured())
            {
                Debug.Log("No fight recorded.");
                OnKingdomLoaded?.Invoke();
                return; // For now, the kingdom loader only needs to behave after a fight.
            }

            AdjustKingdomAfterFight();
            _gameStateManager.CleanPostFightData();
            Debug.Log("Kingdom loaded.");
            OnKingdomLoaded?.Invoke();
        }

        #region Last kingdom state restoration

        /// <summary>
        ///     If kingdom cell occupiers are placed directly in the scene for dev purposes, destroy them.
        /// </summary>
        private void DestroyDevOccupiers()
        {
            FindObjectsOfType<KingdomCellOccupier>().ToList()
                .ForEach(occupier => DestroyImmediate(occupier.gameObject));
        }

        /// <summary>
        ///     Load the entities groups and interest points as they were before the fight.
        /// </summary>
        private void LoadKingdomAsBeforeFight()
        {
            KingdomState kingdomState = _gameStateManager.GetKingdomState();
            foreach (EntitiesGroupData enemiesGroupData in kingdomState.enemiesGroupsData)
                _respawnedEnemiesGroups.Add(
                    EntitiesGroupBuilder.Instance.BuildEntitiesGroup(enemiesGroupData, kingdomHexGrid));

            foreach (InterestPointData interestPointData in kingdomState.interestPointsData)
                InterestPointBuilder.Instance.BuildInterestPoint(interestPointData, kingdomHexGrid);
        }

        #endregion

        #region Hero group spawn & restoration

        private void FirstSpawnHeroGroup()
        {
            // Spawn an empty entities group prefab
            GameObject entitesGroupGo = WorldGameObjectInstantiator.Instance.Instantiate(
                EntitiesGroupBuilder.Instance.BlankEntitiesGroupPrefab
            );
            _respawnedHeroGroup = entitesGroupGo.GetComponent<EntitiesGroup>();
            _respawnedHeroGroup.name = "HeroGroup";

            // Spawn the hero entities
            List<Entity> heroGroupEntities = new();
            foreach (Hero hero in _heroTeam.Heroes)
                heroGroupEntities.Add(
                    Instantiate(
                        hero.EntityConfiguration.KingdomEntityPrefab,
                        _respawnedHeroGroup.transform
                    ).GetComponent<Entity>()
                );

            // Configure the hero group with the hero entities
            _respawnedHeroGroup.UpdateEntities(heroGroupEntities.ToArray());
            _respawnedHeroGroup.movePoints = 10; // * For now, we give the hero group 10 move points.
            KingdomCell kingdomCell = kingdomHexGrid.Cells[new Vector2Int(0, 0)] as KingdomCell;
            if (kingdomCell != null) kingdomCell.SetOccupier(_respawnedHeroGroup);
            AttachCameraToHeroGroup();
        }

        private void RestoreHeroGroup()
        {
            // Restore the hero group as lastly saved
            _respawnedHeroGroup =
                EntitiesGroupBuilder.Instance.BuildEntitiesGroup(_gameStateManager.GetKingdomState().heroGroupData,
                    kingdomHexGrid);
            _respawnedHeroGroup.name = "HeroGroup";

            // Adjust the entities depending on the hero team state
            foreach (Hero hero in _heroTeam.Heroes)
            {
                Entity entity = _respawnedHeroGroup.Entities.ToList()
                    .Find(entity => entity.EntityConfiguration == hero.EntityConfiguration);
                entity.IsDead = hero.IsDead();
            }

            // If the hero group is dead, respawn it
            if (_respawnedHeroGroup.Entities.All(entity => entity.IsDead))
            {
                RespawnHeroGroup();
                AttachCameraToHeroGroup();
                return;
            }

            // Otherwise, update the displayed entity if it's dead
            if (_respawnedHeroGroup.GetDisplayedEntity().IsDead)
                _respawnedHeroGroup.UpdateDisplayedEntity(_respawnedHeroGroup.GetRandomAliveEntity());

            AttachCameraToHeroGroup();
        }

        private void RespawnHeroGroup()
        {
            _respawnedHeroGroup.cell = kingdomHexGrid.Cells[new Vector2Int(0, 0)] as KingdomCell;
            _respawnedHeroGroup.Entities.ToList().ForEach(entity => entity.IsDead = false);
            _heroTeam.FullHealTeam();
        }

        private void AttachCameraToHeroGroup()
        {
            camera.Follow = _respawnedHeroGroup.CameraAnchor;
            camera.LookAt = _respawnedHeroGroup.CameraAnchor;
        }

        #endregion

        #region Post fight adjustments

        /// <summary>
        ///     Adjust the kingdom after a fight => destroy enemies group that lost, update enemies group that won.
        /// </summary>
        private void AdjustKingdomAfterFight()
        {
            PostFightData postFightData = _gameStateManager.GetPostFightData();
            Dictionary<string, PostFightFighterState> enemiesPostFightState =
                SElementToValue<string, PostFightFighterState>.GetDictionaryFromArray(
                    postFightData.enemiesState.ToArray()
                );
            EntitiesGroup foughtEnemiesGroup = GetFoughtEnemiesGroup(enemiesPostFightState);


            // If allies have lost, adjust enemies groups that won
            if (!postFightData.AlliesHaveWon())
                UpdateEntitiesGroupAfterFight(foughtEnemiesGroup, enemiesPostFightState);
            else // Otherwise, destroy enemies group that lost
                DestroyImmediate(foughtEnemiesGroup.gameObject);
        }

        /// <summary>
        ///     Update entities group data after a fight => lasting health, is dead...
        /// </summary>
        /// <param name="entitiesGroupToUpdate">The entities group to update.</param>
        /// <param name="entitiesState">The post fight state of the entities.</param>
        private void UpdateEntitiesGroupAfterFight(EntitiesGroup entitiesGroupToUpdate,
            Dictionary<string, PostFightFighterState> entitiesState)
        {
            foreach (KeyValuePair<string, PostFightFighterState> postFighterData in entitiesState)
            {
                Entity entityToUpdate = entitiesGroupToUpdate.Entities.ToList().Find(
                    entity => entity.SessionId == postFighterData.Key
                );
                entityToUpdate.IsDead = postFighterData.Value.lastingHealth == 0;
                if (entitiesGroupToUpdate.GetDisplayedEntity() == entityToUpdate && entityToUpdate.IsDead)
                    entitiesGroupToUpdate.UpdateDisplayedEntity(entitiesGroupToUpdate.GetRandomAliveEntity());
            }
        }

        private EntitiesGroup GetFoughtEnemiesGroup(Dictionary<string, PostFightFighterState> enemiesPostFightState)
        {
            string enemySessionId = enemiesPostFightState.Keys.First();
            return _respawnedEnemiesGroups.Find(enemiesGroup =>
                enemiesGroup.Entities.Any(entity => entity.SessionId == enemySessionId));
        }

        #endregion
    }
}