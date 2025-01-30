using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using FrostfallSaga.Utils;
using FrostfallSaga.Grid;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.CityBuildings;

namespace FrostfallSaga.Kingdom
{
    public class KingdomLoader : MonoBehaviour
    {
        public Action onKingdomLoaded;

        [SerializeField] private HexGrid _grid;
        [SerializeField] CinemachineVirtualCamera _camera;

        private GameStateManager _gameStateManager;
        private EntitiesGroup _respawnedHeroGroup;
        private readonly List<EntitiesGroup> _respawnedEnemiesGroups = new();
        private readonly List<CityBuilding> _cityBuildings = new();

        private void Start()
        {
            if (_gameStateManager.IsFirstSceneLaunch())
            {
                Debug.Log("First scene launch. No kingdom to load.");
                onKingdomLoaded?.Invoke();
                return;
            }

            Debug.Log("Start loading kingdom.");
            FindObjectsOfType<EntitiesGroup>().ToList().ForEach(entityGroup => DestroyImmediate(entityGroup.gameObject));
            LoadKingdomAsBeforeFight();

            if (!_gameStateManager.HasFightJustOccured())
            {
                Debug.Log("No fight recorded.");
                onKingdomLoaded?.Invoke();
                return; // For now, the kingdom loader only needs to behave after a fight.
            }

            if (!_gameStateManager.GetPostFightData().AlliesHaveWon())
            {
                UpdateEntitiesGroupAfterFight(GetFoughtEnemiesGroup(), isHeroGroup: false);
                Respawn();
                onKingdomLoaded?.Invoke();
                return;
            }

            DestroyImmediate(GetFoughtEnemiesGroup().gameObject);
            UpdateEntitiesGroupAfterFight(_respawnedHeroGroup, isHeroGroup: true);
            _gameStateManager.CleanPostFightData();
            Debug.Log("Kingdom loaded.");
            onKingdomLoaded?.Invoke();
        }

        private void LoadKingdomAsBeforeFight()
        {
            EntitiesGroupBuilder entitiesGroupBuilder = EntitiesGroupBuilder.Instance;
            KingdomState kingdomState = _gameStateManager.GetKingdomState();
            _respawnedHeroGroup = entitiesGroupBuilder.BuildEntitiesGroup(kingdomState.heroGroupData, _grid);
            _respawnedHeroGroup.name = "HeroGroup";
            _camera.Follow = _respawnedHeroGroup.CameraAnchor;
            _camera.LookAt = _respawnedHeroGroup.CameraAnchor;
            foreach (EntitiesGroupData enemiesGroupData in kingdomState.enemiesGroupsData)
            {
                _respawnedEnemiesGroups.Add(entitiesGroupBuilder.BuildEntitiesGroup(enemiesGroupData, _grid));
            }

            // Restore cities
            foreach(CityBuildingData cityBuildingData in kingdomState.cityBuildingsData)
            {
                _cityBuildings.Add(CityBuildingBuilder.Instance.BuildCityBuilding(cityBuildingData, _grid));
            }
        }

        private void AdjustKingdomAfterFight()
        {
            PostFightData postFightData = _gameStateManager.GetPostFightData();

            // If allies have lost, respawn hero group and adjust enemies groups that won
            if (!postFightData.AlliesHaveWon())
            {
                UpdateEntitiesGroupAfterFight(GetFoughtEnemiesGroup(), isHeroGroup: false);
                Respawn();
                onKingdomLoaded?.Invoke();
                return;
            }
            else    // Otherwise, destroy enemies group that lost and adjust hero group
            {
                DestroyImmediate(GetFoughtEnemiesGroup().gameObject);
                UpdateEntitiesGroupAfterFight(_respawnedHeroGroup, isHeroGroup: true);
                postFightData.isActive = false;
            }
        }

        /// <summary>
        /// Update entities group data after a fight => lasting health, is dead...
        /// </summary>
        /// <param name="entitiesGroupToUpdate">The entities group to update.</param>
        /// <param name="isHeroGroup">If the entities group is the hero group or not.</param>
        private void UpdateEntitiesGroupAfterFight(EntitiesGroup entitiesGroupToUpdate, bool isHeroGroup = false)
        {
            PostFightData postFightData = _gameStateManager.GetPostFightData();
            Dictionary<string, PostFightFighterState> entitiesStateDict = SElementToValue<string, PostFightFighterState>.GetDictionaryFromArray(
                isHeroGroup ? postFightData.alliesState.ToArray() : postFightData.enemiesState.ToArray()
            );
            foreach (KeyValuePair<string, PostFightFighterState> postFighterData in entitiesStateDict)
            {
                Entity entityToUpdate = entitiesGroupToUpdate.Entities.ToList().Find(
                    entity => entity.SessionId == postFighterData.Key
                );
                entityToUpdate.IsDead = postFighterData.Value.lastingHealth == 0;
                if (entitiesGroupToUpdate.GetDisplayedEntity() == entityToUpdate && entityToUpdate.IsDead)
                {
                    entitiesGroupToUpdate.UpdateDisplayedEntity(entitiesGroupToUpdate.GetRandomAliveEntity());
                }
            }
        }

        private EntitiesGroup GetFoughtEnemiesGroup()
        {
            return _respawnedEnemiesGroups.Find(enemiesGroup => enemiesGroup.cell == _respawnedHeroGroup.cell);
        }

        private void Respawn()
        {
            _respawnedHeroGroup.cell = _grid.CellsByCoordinates[new(0, 0)] as KingdomCell;
            _respawnedHeroGroup.Entities.ToList().ForEach(entity => entity.IsDead = false);
        }

        #region Setup & tear down

        private void Awake()
        {
            if (_grid == null)
            {
                _grid = FindObjectOfType<HexGrid>();
            }
            if (_grid == null)
            {
                Debug.LogError("No grid found. Can't know where to spawn the entities groups.");
                return;
            }
            _gameStateManager = GameStateManager.Instance;
        }

        #endregion
    }
}