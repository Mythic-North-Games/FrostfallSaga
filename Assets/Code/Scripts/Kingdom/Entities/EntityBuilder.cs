using UnityEngine;

namespace FrostfallSaga.Kingdom.Entities
{
    /// <summary>
    /// Helps saving and building entities at runtime for the kingdom scene.
    /// </summary>
    public class EntityBuilder : MonoBehaviour
    {
        [SerializeField] private string EntityConfigurationsBaseResourcePath;

        public Entity BuildEntity(EntityData entityData)
        {
            EntityConfigurationSO entityConfiguration = Resources.Load<EntityConfigurationSO>(entityData.entityConfigurationResourcePath);
            GameObject entityGO = Instantiate(entityConfiguration.EntityPrefab);
            Entity entity = entityGO.GetComponent<Entity>();
            entity.Setup(entityConfiguration, entityData.sessionId, entityData.isDead);
            return entity;
        }

        public EntityData ExtractEntityDataFromEntity(Entity entity)
        {
            return new()
            {
                sessionId = entity.SessionId,
                isDead = entity.IsDead,
                entityConfigurationResourcePath = $"{EntityConfigurationsBaseResourcePath}/{entity.EntityConfiguration.EntityID}"
            };
        }
    }
}