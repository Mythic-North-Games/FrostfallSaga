using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using FrostfallSaga.Utils;
using FrostfallSaga.Grid;
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

        private KingdomState _kingdomState;
        private PostFightData _postFightData;
        private EntitiesGroupBuilder _entitiesGroupBuilder;
        private CityBuildingBuilder _cityBuildingBuilder;
        private EntitiesGroup _respawnedHeroGroup;
        private readonly List<EntitiesGroup> _respawnedEnemiesGroups = new();
        private readonly List<CityBuilding> _cityBuildings = new();

        private void Start()
        {
            // Check if kingdom state is filled to know if it's the first launch or not
            if (_kingdomState.heroGroupData == null)
            {
                Debug.Log("First kingdom launch.");
                onKingdomLoaded?.Invoke();
                return;
            }

            // Load kingdom from kingdom state
            Debug.Log("Start loading kingdom.");
            LoadKingdomFromKingdomState();
            Debug.Log("Kingdom loaded as before scene change.");

            // Check if a fight has been recorded
            if (!_postFightData.isActive)
            {
                Debug.Log("No fight recorded.");
                onKingdomLoaded?.Invoke();
                return;
            }
            else
            {
                Debug.Log("Fight recorded. Adjust kingdom after fight.");
                AdjustKingdomAfterFight();
                Debug.Log("Kingdom adjusted after fight.");
                onKingdomLoaded?.Invoke();
            }
        }

        private void LoadKingdomFromKingdomState()
        {
            // Destroy dev entities groups already in scene
            FindObjectsOfType<EntitiesGroup>().ToList().ForEach(entityGroup => DestroyImmediate(entityGroup.gameObject));
            
            // Restore hero group
            _respawnedHeroGroup = _entitiesGroupBuilder.BuildEntitiesGroup(_kingdomState.heroGroupData, _grid);
            _respawnedHeroGroup.name = "HeroGroup";
            _camera.Follow = _respawnedHeroGroup.CameraAnchor;
            _camera.LookAt = _respawnedHeroGroup.CameraAnchor;

            // Restore enemies groups
            foreach (EntitiesGroupData enemiesGroupData in _kingdomState.enemiesGroupsData)
            {
                _respawnedEnemiesGroups.Add(_entitiesGroupBuilder.BuildEntitiesGroup(enemiesGroupData, _grid));
            }

            // Restore cities
            foreach(CityBuildingData cityBuildingData in _kingdomState.cityBuildingsData)
            {
                _cityBuildings.Add(_cityBuildingBuilder.BuildCityBuilding(cityBuildingData, _grid));
            }
        }

        private void AdjustKingdomAfterFight()
        {
            // If allies have lost, respawn hero group and adjust enemies groups that won
            if (!_postFightData.AlliesHaveWon())
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
                _postFightData.isActive = false;
            }
        }

        /// <summary>
        /// Update entities group data after a fight => lasting health, is dead...
        /// </summary>
        /// <param name="entitiesGroupToUpdate">The entities group to update.</param>
        /// <param name="isHeroGroup">If the entities group is the hero group or not.</param>
        private void UpdateEntitiesGroupAfterFight(EntitiesGroup entitiesGroupToUpdate, bool isHeroGroup = false)
        {
            Dictionary<string, PostFightFighterState> entitiesStateDict = SElementToValue<string, PostFightFighterState>.GetDictionaryFromArray(
                isHeroGroup ? _postFightData.alliesState.ToArray() : _postFightData.enemiesState.ToArray()
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

            if (_entitiesGroupBuilder == null)
            {
                _entitiesGroupBuilder = FindObjectOfType<EntitiesGroupBuilder>();
            }
            if (_entitiesGroupBuilder == null)
            {
                Debug.LogError("No entities group builder found. Can't re-generate existing entities groups from fight if there are so.");
                return;
            }

            _kingdomState = KingdomState.Instance;
            _postFightData = PostFightData.Instance;
            _entitiesGroupBuilder = EntitiesGroupBuilder.Instance;
            _cityBuildingBuilder = CityBuildingBuilder.Instance;
        }

        #endregion
    }
}