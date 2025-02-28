using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;

namespace FrostfallSaga.Kingdom.EntitiesGroupsSpawner
{
    public class EntitiesGroupsSpawner : MonoBehaviour
    {
        public Action<EntitiesGroup> onEntitiesGroupSpawned;

        [field: SerializeField] public GameObject EntitiesGroupPrefab { get; private set; }
        [field: SerializeField] public GameObject[] SpawnableEntities { get; private set; }
        [field: SerializeField, Range(0, 1)] public float SpawnChance { get; private set; }
        [field: SerializeField] public int MaxNoSpawnInARow { get; private set; }
        [field: SerializeField] public int MaxEntitiesGroupsOnMap { get; private set; }
        [field: SerializeField] public string BaseGroupName { get; private set; }

        [SerializeField] private AHexGrid _grid;
        [SerializeField] private KingdomLoader _kingdomLoader;
        private int _noSpawnInARow;
        private List<EntitiesGroup> _spawnedEntitiesGroups;

        private void Start()
        {
            _noSpawnInARow = 0;
            _spawnedEntitiesGroups = new();
        }

        private void OnKingdomLoaded()
        {
            _spawnedEntitiesGroups = FindObjectsOfType<EntitiesGroup>().ToList().FindAll(entitiesGroup => entitiesGroup.name != "HeroGroup");
            for (int i = 0; i < _spawnedEntitiesGroups.Count; i++)
            {
                _spawnedEntitiesGroups[i].name = $"{BaseGroupName}{i}";
            }
        }

        public void TrySpawnEntitiesGroup()
        {
            if (_spawnedEntitiesGroups.Count == MaxEntitiesGroupsOnMap)
            {
                throw new ImpossibleSpawnException("Max objects on map reached.");
            }

            if (_noSpawnInARow == MaxNoSpawnInARow)
            {
                SpawnEntitiesGroup();
            }
            else
            {
                _noSpawnInARow++;
            }
        }

        public void OnEntitiesGroupDestroyed(EntitiesGroup destroyedEntitiesGroup)
        {
            _spawnedEntitiesGroups.Remove(destroyedEntitiesGroup);
        }

        private void SpawnEntitiesGroup()
        {
            KingdomCell[] availableCellsForSpawn = GetAvailableCellsForSpawn();
            if (availableCellsForSpawn.Length == 0)
            {
                throw new ImpossibleSpawnException("No available cells for spawn found.");
            }

            KingdomCell cellToSpawnTo = Randomizer.GetRandomElementFromArray(availableCellsForSpawn);
            GameObject spawnedEntitiesGroupPrefab = WorldGameObjectInstantiator.Instance.Instantiate(EntitiesGroupPrefab);
            spawnedEntitiesGroupPrefab.name = $"{BaseGroupName}{_spawnedEntitiesGroups.Count}";
            EntitiesGroup spawnedEtitiesGroup = spawnedEntitiesGroupPrefab.GetComponent<EntitiesGroup>();
            spawnedEtitiesGroup.UpdateEntities(EntitiesGroup.GenerateRandomEntities(SpawnableEntities));
            spawnedEtitiesGroup.TeleportToCell(cellToSpawnTo);
            _spawnedEntitiesGroups.Add(spawnedEtitiesGroup);
            _noSpawnInARow = 0;
            onEntitiesGroupSpawned?.Invoke(spawnedEtitiesGroup);
        }

        private KingdomCell[] GetAvailableCellsForSpawn()
        {
            KingdomCell[] kingdomCells = Array.ConvertAll(
                _grid.GetCells(),
                cell => cell as KingdomCell
            );
            return kingdomCells.Where(cell => cell.IsFree()).ToArray();
        }

        #region Setup and tear down
        private void Awake()
        {
            if (_grid == null)
            {
                _grid = FindObjectOfType<AHexGrid>();
            }
            if (_grid == null)
            {
                Debug.LogError("No hex grid found on scene. Can't spawn objects.");
                return;
            }

            if (EntitiesGroupPrefab == null)
            {
                Debug.LogError("No prefab to spawn given.");
                return;
            }

            if (_kingdomLoader == null)
            {
                _kingdomLoader = FindObjectOfType<KingdomLoader>();
            }
            if (_kingdomLoader == null)
            {
                Debug.LogError("No kingdom loader found. Won't be able to correctly configure spawner after fight.");
                return;
            }

            _kingdomLoader.onKingdomLoaded += OnKingdomLoaded;
        }
        #endregion
    }
}