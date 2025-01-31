using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    /// Helps saving and building entities at runtime for the kingdom scene.
    /// </summary>
    public class EntitiesGroupBuilder : MonoBehaviourPersistingSingleton<EntitiesGroupBuilder>
    {
        private readonly EntityBuilder _entityBuilder = new();
        private GameObject _blankEntitiesGroupPrefab;

        public EntitiesGroup BuildEntitiesGroup(EntitiesGroupData entitiesGroupData, HexGrid grid)
        {
            GameObject entitiesGroupPrefab = WorldGameObjectInstantiator.Instance.Instantiate(_blankEntitiesGroupPrefab);
            EntitiesGroup entitiesGroup = entitiesGroupPrefab.GetComponent<EntitiesGroup>();
            List<Entity> entities = new();
            entitiesGroupData.entitiesData.ToList().ForEach(entityData => entities.Add(_entityBuilder.BuildEntity(entityData)));
            entitiesGroup.UpdateEntities(entities.ToArray());
            entitiesGroup.UpdateDisplayedEntity(entities.Find(entity => entity.SessionId == entitiesGroupData.displayedEntitySessionId));
            entitiesGroup.movePoints = entitiesGroupData.movePoints;
            entitiesGroup.TeleportToCell(grid.CellsByCoordinates[new(entitiesGroupData.cellX, entitiesGroupData.cellY)] as KingdomCell);
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
                displayedEntitySessionId = entitiesGroup.GetDisplayedEntity().SessionId
            };
        }

        private EntityData[] GetEntitiesData(EntitiesGroup entitiesGroup)
        {
            List<EntityData> entitiesData = new();
            entitiesGroup.Entities.ToList().ForEach(entity => entitiesData.Add(_entityBuilder.ExtractEntityDataFromEntity(entity)));
            return entitiesData.ToArray();
        }

        private void Start()
        {
            _blankEntitiesGroupPrefab = Resources.Load<GameObject>("Prefabs/EntitiesGroups/EmptyEntitiesGroup");
        }
    }
}