using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    ///     Represents a group of entities inside the kingdom grid.
    ///     It has one entity displayed that can be changed and it can move.
    /// </summary>
    public class EntitiesGroup : KingdomCellOccupier
    {
        public int movePoints;
        [field: SerializeField] public Transform CameraAnchor { get; private set; }
        private Entity _displayedEntity;
        public Action<EntitiesGroup, KingdomCell> onEntityGroupMoved;
        public Entity[] Entities { get; protected set; }

        private void Start()
        {
            Entities = GetComponentsInChildren<Entity>();
            if (Entities == null || Entities.Length == 0) return;
            if (cell == null)
                try
                {
                    KingdomCell _tryToGetStartCell = GameObject.Find("Cell[0;0]").GetComponent<KingdomCell>();
                    if (_tryToGetStartCell.IsFree()) cell = _tryToGetStartCell;
                }
                catch
                {
                    Debug.LogError("Entity group " + name + " does not have a cell.");
                    return;
                }

            if (_displayedEntity == null) UpdateDisplayedEntity(GetRandomAliveEntity());

            transform.position = cell.GetCenter();
        }

        public void MoveToCell(KingdomCell targetCell, bool isLastMove)
        {
            _displayedEntity.MovementController.Move(cell, targetCell, isLastMove);
            if (cell != null) cell.SetOccupier(null);
        }

        public void TeleportToCell(KingdomCell targetCell)
        {
            _displayedEntity.MovementController.TeleportToCell(targetCell);
            if (cell != null) cell.SetOccupier(null);
            cell = targetCell;
            cell.SetOccupier(this);
        }

        private void OnMoveEnded(Cell destinationCell)
        {
            KingdomCell kingdomCell = destinationCell as KingdomCell;
            cell = kingdomCell;
            cell.SetOccupier(this);
            onEntityGroupMoved?.Invoke(this, cell);
        }

        public Entity GetDisplayedEntity()
        {
            return _displayedEntity;
        }

        public void UpdateEntities(Entity[] newMembers)
        {
            Entities = new List<Entity>(newMembers).ToArray();
            Entities.ToList().ForEach(entity =>
            {
                entity.transform.parent = transform;
                entity.transform.localPosition = new Vector3(0, 0, 0);
            });
            UpdateDisplayedEntity(GetRandomAliveEntity());
        }

        public void UpdateDisplayedEntity(Entity newDisplayedEntity)
        {
            if (!Entities.Contains(newDisplayedEntity))
            {
                Debug.LogError("Given entity is not part of the group of the entity group " + name);
                return;
            }

            Entities.ToList().ForEach(entity => entity.HideVisual());
            if (_displayedEntity != null) _displayedEntity.MovementController.onMoveEnded -= OnMoveEnded;

            newDisplayedEntity.ShowVisual();
            newDisplayedEntity.GetComponentInChildren<EntityVisualMovementController>().UpdateParentToMove(gameObject);
            newDisplayedEntity.MovementController.onMoveEnded += OnMoveEnded;
            _displayedEntity = newDisplayedEntity;
        }

        public Entity GetRandomAliveEntity()
        {
            return Randomizer.GetRandomElementFromArray(Entities.Where(entity => !entity.IsDead).ToArray());
        }

        public static Entity[] GenerateRandomEntities(
            GameObject[] availableEntityPrefabs,
            int minNumberOfEntities = 1,
            int maxNumberOfEntities = 3
        )
        {
            List<Entity> entities = new();

            while (entities.Count < minNumberOfEntities)
            {
                GameObject entityPrefab = Instantiate(Randomizer.GetRandomElementFromArray(availableEntityPrefabs));
                entities.Add(entityPrefab.GetComponent<Entity>());
            }

            if (entities.Count >= maxNumberOfEntities) return entities.ToArray();

            var placeLeftInTeam = maxNumberOfEntities - minNumberOfEntities;
            for (var i = 0; i < placeLeftInTeam; i++)
                if (Randomizer.GetBooleanOnChance(0.5f))
                {
                    GameObject entityPrefab = Instantiate(Randomizer.GetRandomElementFromArray(availableEntityPrefabs));
                    entities.Add(entityPrefab.GetComponent<Entity>());
                }

            return entities.ToArray();
        }
    }
}