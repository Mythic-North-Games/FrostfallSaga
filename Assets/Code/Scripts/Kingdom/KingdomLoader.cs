using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Kingdom.EntitiesGroups;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomLoader : MonoBehaviour
    {
        public Action onKingdomLoaded;

        [SerializeField] private KingdomDataSO _kingdomData;
        [SerializeField] private PostFightDataSO _postFightData;
        [SerializeField] private HexGrid _grid;
        [SerializeField] private EntitiesGroupBuilder _entitiesGroupBuilder;
        [SerializeField] CinemachineVirtualCamera _camera;

        private EntitiesGroup _respawnedHeroGroup;
        private readonly List<EntitiesGroup> _respawnedEnemiesGroups = new();

        private void Start()
        {
            if (!_postFightData.enabled)
            {
                Debug.Log("No fight recorded.");
                return; // For now, the kingdom loader only needs to behave after a fight.
            }

            Debug.Log("Start loading kingdom.");
            Destroy(FindObjectOfType<EntitiesGroup>().gameObject);
            LoadKingdomAsBeforeFight();
            if (!_postFightData.AlliesHaveWon())
            {
                UpdateEntitiesGroupAfterFight(GetFoughtEnemiesGroup());
                Respawn();
                onKingdomLoaded?.Invoke();
                return;
            }

            DestroyImmediate(GetFoughtEnemiesGroup().gameObject);
            UpdateEntitiesGroupAfterFight(_respawnedHeroGroup);
            _postFightData.enabled = false;
            Debug.Log("Kingdom loaded.");
            onKingdomLoaded?.Invoke();
        }

        private void LoadKingdomAsBeforeFight()
        {
            _respawnedHeroGroup = _entitiesGroupBuilder.BuildEntitiesGroup(_kingdomData.heroGroupData, _grid);
            _respawnedHeroGroup.name = "HeroGroup";
            _camera.Follow = _respawnedHeroGroup.CameraAnchor;
            _camera.LookAt = _respawnedHeroGroup.CameraAnchor;
            foreach (EntitiesGroupData enemiesGroupData in _kingdomData.enemiesGroupsData)
            {
                _respawnedEnemiesGroups.Add(_entitiesGroupBuilder.BuildEntitiesGroup(enemiesGroupData, _grid));
            }
        }

        private void UpdateEntitiesGroupAfterFight(EntitiesGroup entitiesGroupToUpdate)
        {
            foreach (KeyValuePair<string, PostFightFighterState> postFighterData in _postFightData.alliesState)
            {
                Entity entityToUpdate = entitiesGroupToUpdate.Entities.ToList().Find(
                    entity => entity.sessionId == postFighterData.Key
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
            _respawnedHeroGroup.cell = _grid.CellsByCoordinates[new(0, 0)];
            _respawnedHeroGroup.Entities.ToList().ForEach(entity => entity.IsDead = false);
        }

        #region Setup & tear down

        private void Awake()
        {
            if (_kingdomData == null)
            {
                Debug.LogError("No kingdom data given. Can't know how to load the kingdom.");
                return;
            }

            if (_postFightData == null)
            {
                Debug.LogError("No post fight data given. Can't know how to load the kingdom.");
                return;
            }

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
        }

        #endregion
    }
}