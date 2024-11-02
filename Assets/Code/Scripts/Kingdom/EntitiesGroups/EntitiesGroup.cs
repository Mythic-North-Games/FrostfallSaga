using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.EntitiesVisual;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    /// Represents a group of entities inside the kingdom grid.
    /// It has one entity displayed that can be changed and it can move.
    /// </summary>
    public class EntitiesGroup : MonoBehaviour
    {
        public int movePoints;
        public Cell cell;
        [field:SerializeField] public Transform CameraAnchor { get; private set; }
        public Action<EntitiesGroup> onEntityGroupHovered;
        public Action<EntitiesGroup> onEntityGroupUnhovered;
        public Action<EntitiesGroup> onEntityGroupClicked;
        public Action<EntitiesGroup, Cell> onEntityGroupMoved;
        public Entity[] Entities { get; protected set; }
        private Entity _displayedEntity;

        private void Start()
        {
            Entities = GetComponentsInChildren<Entity>();
            if (Entities == null || Entities.Length == 0)
            {
                Debug.LogError("Entity group " + name + " does not have entities");
                return;
            }
            if (cell == null)
            {
                try
                {
                    Cell _tryToGetStartCell = GameObject.Find("Cell[0;0]").GetComponent<Cell>();
                    if (_tryToGetStartCell.IsFree())
                    {
                        cell = _tryToGetStartCell;
                    }
                }
                catch
                {
                    Debug.LogError("Entity group " + name + " does not have a cell.");
                    return;
                }
            }

            if (_displayedEntity == null)
            {
                UpdateDisplayedEntity(GetRandomAliveEntity());
            }

            transform.position = cell.GetCenter();
        }

        public void MoveToCell(Cell targetCell, bool isLastMove)
        {
            _displayedEntity.EntityVisualMovementController.Move(cell, targetCell, isLastMove);
        }

        public void TeleportToCell(Cell targetCell)
        {
            _displayedEntity.EntityVisualMovementController.TeleportToCell(targetCell);
            cell = targetCell;
        }

        private void OnMoveEnded(Cell destinationCell)
        {
            cell = destinationCell;
            onEntityGroupMoved?.Invoke(this, destinationCell);
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
                entity.name = Enum.GetName(typeof(EntityID), entity.EntityConfiguration.EntityID) + entity.sessionId;
                entity.transform.parent = transform;
                entity.transform.localPosition = new(0, 0, 0);
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
            if (_displayedEntity != null)
            {
                _displayedEntity.EntityMouseEventsController.OnElementHover -= OnDisplayedEntityHovered;
                _displayedEntity.EntityMouseEventsController.OnElementUnhover -= OnDisplayedEntityUnhovered;
                _displayedEntity.EntityVisualMovementController.onMoveEnded -= OnMoveEnded;
            }

            newDisplayedEntity.ShowVisual();
            newDisplayedEntity.GetComponentInChildren<EntityVisualMovementController>().UpdateParentToMove(gameObject);
            newDisplayedEntity.EntityMouseEventsController.OnElementHover += OnDisplayedEntityHovered;
            newDisplayedEntity.EntityMouseEventsController.OnElementUnhover += OnDisplayedEntityUnhovered;
            newDisplayedEntity.EntityMouseEventsController.OnLeftMouseUp += OnDisplayedEntityClicked;
            newDisplayedEntity.EntityVisualMovementController.onMoveEnded += OnMoveEnded;
            _displayedEntity = newDisplayedEntity;
        }

        public Entity GetRandomAliveEntity()
        {
            return Randomizer.GetRandomElementFromArray(Entities.Where(entity => !entity.IsDead).ToArray());
        }

        private void OnDisplayedEntityHovered(Entity hoveredEntity)
        {
            onEntityGroupHovered?.Invoke(this);
        }

        private void OnDisplayedEntityUnhovered(Entity unhoveredEntity)
        {
            onEntityGroupUnhovered?.Invoke(this);
        }

        private void OnDisplayedEntityClicked(Entity clickedEntity)
        {
            onEntityGroupClicked?.Invoke(this);
        }

        private void OnDisable()
        {
            if (_displayedEntity)
            {
                _displayedEntity.EntityMouseEventsController.OnElementHover -= OnDisplayedEntityHovered;
                _displayedEntity.EntityMouseEventsController.OnElementUnhover -= OnDisplayedEntityUnhovered;
                _displayedEntity.EntityMouseEventsController.OnLeftMouseUp -= OnDisplayedEntityClicked;
            }
        }

        public static Entity[] GenerateRandomEntities(
            GameObject[] availableEntitiesPrefab,
            int minNumberOfEntities = 1,
            int maxNumberOfEntities = 3
        )
        {
            List<Entity> entities = new();

            while (entities.Count < minNumberOfEntities)
            {
                GameObject entityPrefab = Instantiate(Randomizer.GetRandomElementFromArray(availableEntitiesPrefab));
                entities.Add(entityPrefab.GetComponent<Entity>());
            }

            if (entities.Count >= maxNumberOfEntities)
            {
                return entities.ToArray();
            }

            int placeLeftInTeam = maxNumberOfEntities - minNumberOfEntities;
            for (int i = 0; i < placeLeftInTeam; i++)
            {
                if (Randomizer.GetBooleanOnChance(0.5f))
                {
                    GameObject entityPrefab = Instantiate(Randomizer.GetRandomElementFromArray(availableEntitiesPrefab));
                    entities.Add(entityPrefab.GetComponent<Entity>());
                }
            }

            return entities.ToArray();
        }
    }
}
