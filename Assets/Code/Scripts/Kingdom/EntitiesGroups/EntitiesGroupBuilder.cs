using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Kingdom.Entities;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    ///     Helps saving and building entities at runtime for the kingdom scene.
    /// </summary>
    public class EntitiesGroupBuilder : MonoBehaviourPersistingSingleton<EntitiesGroupBuilder>
    {
        private EntityBuilder _entityBuilder;

        static EntitiesGroupBuilder()
        {
            PersistAcrossScenes = false;
        }

        private GameObject _blankEntitiesGroupPrefab;
        
        public EntitiesGroup BuildEntitiesGroup(EntitiesGroupData entitiesGroupData, KingdomCell kingdomCell)
        {
            GameObject entitiesGroupPrefab =
                WorldGameObjectInstantiator.Instance.Instantiate(_blankEntitiesGroupPrefab);
            EntitiesGroup entitiesGroup = entitiesGroupPrefab.GetComponent<EntitiesGroup>();
            List<Entity> entities = new();
            entitiesGroupData.entitiesData.ToList()
                .ForEach(entityData => entities.Add(_entityBuilder.BuildEntity(entityData)));

            entitiesGroup.UpdateEntities(entities.ToArray());
            entitiesGroup.UpdateDisplayedEntity(entities.Find(entity =>
                entity.SessionId == entitiesGroupData.displayedEntitySessionId));

            entitiesGroup.name = entitiesGroupData.entitiesGroupName;
            entitiesGroup.movePoints = entitiesGroupData.movePoints;

            entitiesGroup.TeleportToCell(kingdomCell);
            return entitiesGroup;
        }

        public EntitiesGroup BuildEntitiesGroup(List<EntityConfigurationSO> entitiesConfiguration,
            KingdomCell kingdomCell, string entityGroupName, int movePoints)
        {
            GameObject entitesGroupPrefab =
                WorldGameObjectInstantiator.Instance.Instantiate(Instance._blankEntitiesGroupPrefab);

            EntitiesGroup entitiesGroup = entitesGroupPrefab.GetComponent<EntitiesGroup>();
            entitiesGroup.name = entityGroupName;

            List<Entity> entities = new();
            foreach (EntityConfigurationSO entityConfiguration in entitiesConfiguration)
                entities.Add(_entityBuilder.BuildEntity(entityConfiguration));
            entitiesGroup.UpdateEntities(entities.ToArray());

            entitiesGroup.UpdateDisplayedEntity(entitiesGroup.GetRandomAliveEntity());
            
            entitiesGroup.movePoints = movePoints;
            
            entitiesGroup.TeleportToCell(kingdomCell);

            return entitiesGroup;
        }

        public EntitiesGroupData ExtractEntitiesGroupDataFromEntiesGroup(EntitiesGroup entitiesGroup)
        {
            return new EntitiesGroupData
            {
                entitiesGroupName = entitiesGroup.name,
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
            entitiesGroup.Entities.ToList()
                .ForEach(entity => entitiesData.Add(_entityBuilder.ExtractEntityDataFromEntity(entity)));
            return entitiesData.ToArray();
        }

        protected override void Init()
        {
            _blankEntitiesGroupPrefab = Resources.Load<GameObject>("Prefabs/EntitiesGroups/EmptyEntitiesGroup");
            _entityBuilder = new EntityBuilder(Resources.Load<GameObject>("Prefabs/Entities/Entity"));
        }
    }
}