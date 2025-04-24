using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Kingdom.EntitiesGroupsSpawner
{
    public class EntitiesGroupsSpawner : MonoBehaviour
    {
        [field: SerializeField] public GameObject EntitiesGroupPrefab { get; private set; }
        [field: SerializeField] public GameObject[] SpawnableEntities { get; private set; }

        [field: SerializeField] public int MaxNoSpawnInARow { get; private set; }
        [field: SerializeField] public int MaxEntitiesGroupsOnMap { get; private set; }
        [field: SerializeField] public string BaseGroupName { get; private set; }

        [SerializeField] private KingdomHexGrid grid;
        [SerializeField] private KingdomLoader kingdomLoader;
        private int _noSpawnInARow;
        private List<EntitiesGroup> _spawnedEntitiesGroups;
        public Action<EntitiesGroup> OnEntitiesGroupSpawned;

        #region Setup and tear down

        private void Awake()
        {
            if (grid == null) grid = FindObjectOfType<KingdomHexGrid>();
            if (grid == null)
            {
                Debug.LogError("No hex grid found on scene. Can't spawn objects.");
                return;
            }

            if (EntitiesGroupPrefab == null)
            {
                Debug.LogError("No prefab to spawn given.");
                return;
            }

            if (kingdomLoader == null) kingdomLoader = FindObjectOfType<KingdomLoader>();
            if (kingdomLoader == null)
            {
                Debug.LogError("No kingdom loader found. Won't be able to correctly configure spawner after fight.");
                return;
            }

            kingdomLoader.onKingdomLoaded += OnKingdomLoaded;
        }

        #endregion

        private void Start()
        {
            _noSpawnInARow = 0;
            _spawnedEntitiesGroups = new List<EntitiesGroup>();
        }

        private void OnKingdomLoaded()
        {
            _spawnedEntitiesGroups = FindObjectsOfType<EntitiesGroup>().ToList()
                .FindAll(entitiesGroup => entitiesGroup.name != "HeroGroup");
            for (int i = 0; i < _spawnedEntitiesGroups.Count; i++)
                _spawnedEntitiesGroups[i].name = $"{BaseGroupName}{i}";
        }

        public void TrySpawnEntitiesGroup()
        {
            if (_spawnedEntitiesGroups.Count == MaxEntitiesGroupsOnMap)
                throw new ImpossibleSpawnException("Max objects on map reached.");

            if (_noSpawnInARow == MaxNoSpawnInARow)
                SpawnEntitiesGroup();
            else
                _noSpawnInARow++;
        }

        public void OnEntitiesGroupDestroyed(EntitiesGroup destroyedEntitiesGroup)
        {
            _spawnedEntitiesGroups.Remove(destroyedEntitiesGroup);
        }

        private void SpawnEntitiesGroup()
        {
            KingdomCell[] availableCellsForSpawn = GetAvailableCellsForSpawn();
            if (availableCellsForSpawn.Length == 0)
                throw new ImpossibleSpawnException("No available cells for spawn found.");

            KingdomCell cellToSpawnTo = Randomizer.GetRandomElementFromArray(availableCellsForSpawn);
            GameObject spawnedEntitiesGroupPrefab =
                WorldGameObjectInstantiator.Instance.Instantiate(EntitiesGroupPrefab);
            spawnedEntitiesGroupPrefab.name = $"{BaseGroupName}{_spawnedEntitiesGroups.Count}";
            EntitiesGroup spawnedEtitiesGroup = spawnedEntitiesGroupPrefab.GetComponent<EntitiesGroup>();
            spawnedEtitiesGroup.UpdateEntities(EntitiesGroup.GenerateRandomEntities(SpawnableEntities));
            spawnedEtitiesGroup.TeleportToCell(cellToSpawnTo);
            _spawnedEntitiesGroups.Add(spawnedEtitiesGroup);
            _noSpawnInARow = 0;
            OnEntitiesGroupSpawned?.Invoke(spawnedEtitiesGroup);
        }

        private KingdomCell[] GetAvailableCellsForSpawn()
        {
            KingdomCell[] kingdomCells = Array.ConvertAll(
                grid.GetCells(),
                cell => cell as KingdomCell
            );
            return kingdomCells.Where(cell => cell.IsFree()).ToArray();
        }
    }
}