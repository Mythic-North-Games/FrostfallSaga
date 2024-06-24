using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    /// Represents a group of entities inside the kingdom grid.
    /// It has one entity displayed that can be changed and it can move.
    /// </summary>
    public class EntitiesGroup : MonoBehaviour
    {
        public int MovePoints;
        public Cell Cell;
        public Action<EntitiesGroup> OnEntityGroupHovered;
        public Action<EntitiesGroup> OnEntityGroupUnhovered;
        public Action<EntitiesGroup, Cell> OnEntityGroupMoved;
        private Entity[] entities;
        private Entity _displayedEntity;

        private void Start()
        {
            entities = GetComponentsInChildren<Entity>();
            if (entities == null || entities.Length == 0)
            {
                Debug.LogError("Entity group " + name + " does not have entities");
                gameObject.SetActive(false);
                return;
            }
            if (Cell == null)
            {
                Debug.LogError("Entity group " + name + " does not have a cell.");
                gameObject.SetActive(false);
                return;
            }

            if (_displayedEntity == null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (i == 0)
                    {
                        UpdateDisplayedEntity(entities[i]);
                    }
                    else
                    {
                        entities[i].HideVisual();
                    }
                }
            }

            transform.position = Cell.GetCenter();
        }

        public void MoveToCell(Cell targetCell, bool isLastMove)
        {
            _displayedEntity.EntityVisualMovementController.Move(Cell, targetCell, isLastMove);
        }

        private void OnMoveEnded(Cell destinationCell)
        {
            Cell = destinationCell;
            OnEntityGroupMoved?.Invoke(this, destinationCell);
        }

        public Entity GetDisplayedEntity()
        {
            return _displayedEntity;
        }

        public void UpdateDisplayedEntity(Entity newDisplayedEntity)
        {
            if (!entities.Contains(newDisplayedEntity))
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

                newDisplayedEntity.EntityMouseEventsController.OnElementHover += OnDisplayedEntityHovered;
                newDisplayedEntity.EntityMouseEventsController.OnElementUnhover += OnDisplayedEntityUnhovered;
                newDisplayedEntity.EntityVisualMovementController.onMoveEnded += OnMoveEnded;
                newDisplayedEntity.ShowVisual();
                _displayedEntity = newDisplayedEntity;
            }
        }

        private void OnDisplayedEntityHovered(Entity hoveredEntity)
        {
            OnEntityGroupHovered?.Invoke(this);
        }

        private void OnDisplayedEntityUnhovered(Entity hoveredEntity)
        {
            OnEntityGroupUnhovered?.Invoke(this);
        }

        private void OnDisable()
        {
            if (_displayedEntity)
            {
                _displayedEntity.EntityMouseEventsController.OnElementHover -= OnDisplayedEntityHovered;
                _displayedEntity.EntityMouseEventsController.OnElementUnhover -= OnDisplayedEntityUnhovered;
            }
        }
    }
}
