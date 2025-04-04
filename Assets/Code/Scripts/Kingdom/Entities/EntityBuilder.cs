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
        public Entity BuildEntity(EntityData entityData)
        {
            GameObject entityGO =
                WorldGameObjectInstantiator.Instance.Instantiate(entityData.entityConfiguration.KingdomEntityPrefab);
            Entity entity = entityGO.GetComponent<Entity>();
            entity.Setup(entityData.sessionId, entityData.isDead);
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