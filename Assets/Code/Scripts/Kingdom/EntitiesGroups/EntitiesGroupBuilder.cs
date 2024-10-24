using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    /// Helps saving and building entities at runtime for the kingdom scene.
    /// </summary>
    public class EntitiesGroupBuilder : MonoBehaviour
    {
        [SerializeField] private EntityBuilder _entityBuilder;
        [SerializeField] private GameObject _blankEntitiesGroupPrefab;

        public EntitiesGroup BuildEntitiesGroup(EntitiesGroupData entitiesGroupData, HexGrid grid)
        {
            GameObject entitiesGroupPrefab = Instantiate(_blankEntitiesGroupPrefab);
            EntitiesGroup entitiesGroup = entitiesGroupPrefab.GetComponent<EntitiesGroup>();
            List<Entity> entities = new();
            entitiesGroupData.entitiesData.ToList().ForEach(entityData => entities.Add(_entityBuilder.BuildEntity(entityData)));
            entitiesGroup.UpdateEntities(entities.ToArray());
            entitiesGroup.UpdateDisplayedEntity(entities.Find(entity => entity.sessionId == entitiesGroupData.displayedEntitySessionId));
            entitiesGroup.movePoints = entitiesGroupData.movePoints;
            entitiesGroup.TeleportToCell(grid.CellsByCoordinates[new(entitiesGroupData.cellX, entitiesGroupData.cellY)]);
            return entitiesGroup;
        }

        public EntitiesGroupData ExtractEntitiesGroupDataFromEntiesGroup(EntitiesGroup entitiesGroup)
        {
            return new()
            {
                movePoints = entitiesGroup.movePoints,
                cellX = entitiesGroup.cell.Coordinates.x,
                cellY = entitiesGroup.cell.Coordinates.y,
                entitiesData = GetEntitiesData(entitiesGroup),
                displayedEntitySessionId = entitiesGroup.GetDisplayedEntity().sessionId
            };
        }

        private EntityData[] GetEntitiesData(EntitiesGroup entitiesGroup)
        {
            List<EntityData> entitiesData = new();
            entitiesGroup.Entities.ToList().ForEach(entity => entitiesData.Add(_entityBuilder.ExtractEntityDataFromEntity(entity)));
            return entitiesData.ToArray();
        }

        #region Setup & tear down

        private void Start()
        {
            if (_entityBuilder == null)
            {
                _entityBuilder = FindObjectOfType<EntityBuilder>();
            }

            if (_entityBuilder == null)
            {
                Debug.LogError("EntityBuilder not found. Won't be able to build entities groups.");
            }
        }

        #endregion
    }
}