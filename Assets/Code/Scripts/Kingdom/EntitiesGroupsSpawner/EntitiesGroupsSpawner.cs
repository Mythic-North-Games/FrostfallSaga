using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Core;

namespace FrostfallSaga.Kingdom.EntitiesGroupsSpawner
{
    public class EntitiesGroupsSpawner : MonoBehaviour
    {
        public Action<EntitiesGroup> onEntitiesGroupSpawned;

        [field: SerializeField] public GameObject EntitiesGroupPrefab { get; private set; }
        [field: SerializeField] public EntitiesListSO SpawnableEntities { get; private set; }
        [field: SerializeField, Range(0, 1)] public float SpawnChance { get; private set; }
        [field: SerializeField] public int MaxNoSpawnInARow { get; private set; }
        [field: SerializeField] public int MaxEntitiesGroupsOnMap { get; private set; }
        [field: SerializeField] public string BaseGroupName { get; private set; }

        [SerializeField] private HexGrid _grid;
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

        public void TrySpawnEntitiesGroup(Cell[] prohibitedCells)
        {
            if (_spawnedEntitiesGroups.Count == MaxEntitiesGroupsOnMap)
            {
                throw new ImpossibleSpawnException("Max objects on map reached.");
            }

            if (_noSpawnInARow == MaxNoSpawnInARow)
            {
                SpawnEntitiesGroup(prohibitedCells);
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

        private void SpawnEntitiesGroup(Cell[] prohibitedCells)
        {
            Cell[] availableCellsForSpawn = GetAvailableCellsForSpawn(prohibitedCells);
            if (availableCellsForSpawn.Length == 0)
            {
                throw new ImpossibleSpawnException("No available cells for spawn found.");
            }

            Cell cellToSpawnTo = Randomizer.GetRandomElementFromArray(availableCellsForSpawn);
            GameObject spawnedEntitiesGroupPrefab = Instantiate(EntitiesGroupPrefab);
            spawnedEntitiesGroupPrefab.name = $"{BaseGroupName}{_spawnedEntitiesGroups.Count}";
            EntitiesGroup spawnedEtitiesGroup = spawnedEntitiesGroupPrefab.GetComponent<EntitiesGroup>();
            spawnedEtitiesGroup.UpdateEntities(EntitiesGroup.GenerateRandomEntities(SpawnableEntities.AvailableEntities));
            spawnedEtitiesGroup.TeleportToCell(cellToSpawnTo);
            _spawnedEntitiesGroups.Add(spawnedEtitiesGroup);
            _noSpawnInARow = 0;
            onEntitiesGroupSpawned?.Invoke(spawnedEtitiesGroup);
        }

        private Cell[] GetAvailableCellsForSpawn(Cell[] prohibitedCells)
        {
            return _grid.GetCells().Where(cell => !prohibitedCells.Contains(cell) && cell.IsFree()).ToArray();
        }

        #region Setup and tear down
        private void OnEnable()
        {
            if (_grid == null)
            {
                _grid = FindObjectOfType<HexGrid>();
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