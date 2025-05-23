using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Kingdom.Entities
{
    /// <summary>
    ///     Helps saving and building entities at runtime for the kingdom scene.
    /// </summary>
    public class EntityBuilder
    {
        private readonly GameObject _basedPrefab;
        
        public EntityBuilder(GameObject basedPrefab)
        {
            _basedPrefab = basedPrefab;
        }

        public Entity BuildEntity(EntityData entityData)
        {
            Entity entity = InstantiateEntity(entityData.entityConfiguration);
            entity.Setup(entityData.entityConfiguration, entityData.sessionId, entityData.isDead);
            return entity;
        }
        
        public Entity BuildEntity(EntityConfigurationSO entityConfiguration)
        {
            Entity entity = InstantiateEntity(entityConfiguration);
            entity.Setup(entityConfiguration);
            return entity;
        }

        private Entity InstantiateEntity(EntityConfigurationSO entityConfiguration)
        {
            GameObject parentObject = WorldGameObjectInstantiator.Instance.Instantiate(_basedPrefab);
            Object.Instantiate(entityConfiguration.CharacterVisual, parentObject.transform);
            Entity entity = parentObject.GetComponent<Entity>();
            return entity;
        }
        
        public EntityData ExtractEntityDataFromEntity(Entity entity)
        {
            return new EntityData
            {
                entityConfiguration = entity.EntityConfiguration,
                sessionId = entity.SessionId,
                isDead = entity.IsDead
            };
        }
    }
}