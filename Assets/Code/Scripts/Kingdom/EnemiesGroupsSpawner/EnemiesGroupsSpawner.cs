using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Core;

namespace FrostfallSaga.Kingdom.EnemiesGroupsSpawner
{
    public class EnemiesGroupsSpawner : MonoBehaviour
    {
        public Action<EntitiesGroup> onEnemiesGroupSpawned;

        [field: SerializeField] public GameObject EnemiesGroupPrefab { get; private set; }
        [field: SerializeField] public EnemyEntitiesListSO SpawnableEntities { get; private set; }
        [field: SerializeField, Range(0, 1)] public float SpawnChance { get; private set; }
        [field: SerializeField] public int MaxNoSpawnInARow { get; private set; }
        [field: SerializeField] public int MaxEnemiesGroupsOnMap { get; private set; }

        [SerializeField] private HexGrid _grid;
        [SerializeField] private KingdomLoader _kingdomLoader;
        private int _noSpawnInARow;
        private List<EntitiesGroup> _spawnedEnemiesGroups;

        private void Start()
        {
            _noSpawnInARow = 0;
            _spawnedEnemiesGroups = new();
        }

        private void OnKingdomLoaded()
        {
            _spawnedEnemiesGroups = FindObjectsOfType<EntitiesGroup>().ToList().FindAll(entitiesGroup => entitiesGroup.name != "HeroGroup");
            for (int i = 0; i < _spawnedEnemiesGroups.Count; i++)
            {
                _spawnedEnemiesGroups[i].name = "EnemiesGroup" + i;
            }
        }

        public void TrySpawnEnemiesGroup(Cell[] prohibitedCells)
        {
            if (_spawnedEnemiesGroups.Count == MaxEnemiesGroupsOnMap)
            {
                throw new ImpossibleSpawnException("Max objects on map reached.");
            }

            if (_noSpawnInARow == MaxNoSpawnInARow)
            {
                SpawnEnemiesGroup(prohibitedCells);
            }
            else
            {
                _noSpawnInARow++;
            }
        }

        public void OnEnemiesGroupDestroyed(EntitiesGroup destroyedEnemiesGroup)
        {
            _spawnedEnemiesGroups.Remove(destroyedEnemiesGroup);
        }

        private void SpawnEnemiesGroup(Cell[] prohibitedCells)
        {
            Cell[] availableCellsForSpawn = GetAvailableCellsForSpawn(prohibitedCells);
            if (availableCellsForSpawn.Length == 0)
            {
                throw new ImpossibleSpawnException("No available cells for spawn found.");
            }

            Cell cellToSpawnTo = Randomizer.GetRandomElementFromArray(availableCellsForSpawn);
            GameObject spawnedEnemiesGroupPrefab = Instantiate(EnemiesGroupPrefab);
            spawnedEnemiesGroupPrefab.name = "EnemiesGroup" + _spawnedEnemiesGroups.Count;
            EntitiesGroup spawnedEnemiesGroup = spawnedEnemiesGroupPrefab.GetComponent<EntitiesGroup>();
            spawnedEnemiesGroup.UpdateEntities(EntitiesGroup.GenerateRandomEntities(SpawnableEntities.AvailableEnemyEntities));
            spawnedEnemiesGroup.TeleportToCell(cellToSpawnTo);
            _spawnedEnemiesGroups.Add(spawnedEnemiesGroup);
            _noSpawnInARow = 0;
            onEnemiesGroupSpawned?.Invoke(spawnedEnemiesGroup);
        }

        private Cell[] GetAvailableCellsForSpawn(Cell[] prohibitedCells)
        {
            return _grid.GetCells().Where(cell => !prohibitedCells.Contains(cell) && cell.IsAccessible).ToArray();
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

            if (EnemiesGroupPrefab == null)
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