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
    public class EntityGroup : MonoBehaviour
    {
        [field: SerializeField] public Entity[] Entities { get; private set; }
        [field: SerializeField] public int MovePoints { get; private set; }
        public Cell Cell;
        public Action<EntityGroup> OnEntityGroupHovered;
        public Action<EntityGroup> OnEntityGroupUnhovered;
        public Action<EntityGroup, Cell> OnEntityGroupMoved;
        private Entity _displayedEntity;

        private void Start()
        {
            Entities = GetComponentsInChildren<Entity>();
            if (Entities == null || Entities.Length == 0)
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

            transform.position = Cell.GetCenter();
        }

        public void MoveToCell(Cell targetCell)
        {
            // TODO: Use movement controller of displayed entity to animate movement
            transform.position = targetCell.GetCenter();
            Cell = targetCell;
            OnEntityGroupMoved?.Invoke(this, targetCell);
        }

        public Entity GetDisplayedEntity()
        {
            return _displayedEntity;
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
                    _displayedEntity.HideVisual();
                }

                newDisplayedEntity.EntityMouseEventsController.OnElementHover += OnDisplayedEntityHovered;
                newDisplayedEntity.EntityMouseEventsController.OnElementUnhover += OnDisplayedEntityUnhovered;
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
