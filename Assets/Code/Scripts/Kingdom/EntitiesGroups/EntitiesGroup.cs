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
        public Action<EntitiesGroup> onEntityGroupHovered;
        public Action<EntitiesGroup> onEntityGroupUnhovered;
        public Action<EntitiesGroup, Cell> onEntityGroupMoved;
        public Entity[] Entities { get; protected set; }
        protected Entity _displayedEntity;

        private void Start()
        {
            Entities = GetComponentsInChildren<Entity>();
            if (Entities == null || Entities.Length == 0)
            {
                Debug.LogError("Entity group " + name + " does not have entities");
                gameObject.SetActive(false);
                return;
            }
            if (cell == null)
            {
                Debug.LogError("Entity group " + name + " does not have a cell.");
                gameObject.SetActive(false);
                return;
            }

            if (_displayedEntity == null)
            {
                for (int i = 0; i < Entities.Length; i++)
                {
                    if (i == 0)
                    {
                        UpdateDisplayedEntity(Entities[i]);
                    }
                    else
                    {
                        Entities[i].HideVisual();
                    }
                }
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
            
            for (int i = 0; i < Entities.Length; i++)
            {
                if (i == 0)
                {
                    UpdateDisplayedEntity(Entities[i]);
                }
                else
                {
                    Entities[i].HideVisual();
                }

                Entities[i].name = Enum.GetName(typeof(EntityType), Entities[i].EntityConfiguration.EntityType) + i;
                Entities[i].transform.parent = transform;
                Entities[i].transform.localPosition = new(0, 0, 0);
            }
            
        }

        public void UpdateDisplayedEntity(Entity newDisplayedEntity)
        {
            if (!Entities.Contains(newDisplayedEntity))
            {
                Debug.LogError("Given entity is not part of the group of the entity group " + name);
                return;
            }

            if (newDisplayedEntity != _displayedEntity)
            {
                if (_displayedEntity != null)
                {
                    _displayedEntity.EntityMouseEventsController.OnElementHover -= OnDisplayedEntityHovered;
                    _displayedEntity.EntityMouseEventsController.OnElementUnhover -= OnDisplayedEntityUnhovered;
                    _displayedEntity.EntityVisualMovementController.onMoveEnded -= OnMoveEnded;
                    _displayedEntity.HideVisual();
                }

                newDisplayedEntity.GetComponentInChildren<EntityVisualMovementController>().UpdateParentToMove(gameObject);
                newDisplayedEntity.EntityMouseEventsController.OnElementHover += OnDisplayedEntityHovered;
                newDisplayedEntity.EntityMouseEventsController.OnElementUnhover += OnDisplayedEntityUnhovered;
                newDisplayedEntity.EntityVisualMovementController.onMoveEnded += OnMoveEnded;
                newDisplayedEntity.ShowVisual();
                _displayedEntity = newDisplayedEntity;
            }
        }

        private void OnDisplayedEntityHovered(Entity hoveredEntity)
        {
            onEntityGroupHovered?.Invoke(this);
        }

        private void OnDisplayedEntityUnhovered(Entity hoveredEntity)
        {
            onEntityGroupUnhovered?.Invoke(this);
        }

        private void OnDisable()
        {
            if (_displayedEntity)
            {
                _displayedEntity.EntityMouseEventsController.OnElementHover -= OnDisplayedEntityHovered;
                _displayedEntity.EntityMouseEventsController.OnElementUnhover -= OnDisplayedEntityUnhovered;
            }
        }

        public static Entity[] GenerateRandomEntities(
            GameObject[] availableEntitiesPrefab, 
            int minNumberOfEntities = 1, 
            int maxNumberOfEntities = 3
        )
        {
            List<Entity> entities = new();

            while(entities.Count < minNumberOfEntities)
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
