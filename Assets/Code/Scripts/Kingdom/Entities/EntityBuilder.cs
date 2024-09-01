using UnityEngine;

namespace FrostfallSaga.Kingdom.Entities
{
    /// <summary>
    /// Helps saving and building entities at runtime for the kingdom scene.
    /// </summary>
    public class EntityBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject BlankEntityPrefab;
        [SerializeField] private string EntityConfigurationsBaseResourcePath;

        public Entity BuildEntity(EntityData entityData)
        {
            GameObject entityPrefab = Instantiate(BlankEntityPrefab);
            Entity entity = entityPrefab.GetComponent<Entity>();
            entity.Setup(entityData);
            return entity;
        }

        public EntityData ExtractEntityDataFromEntity(Entity entity)
        {
            return new()
            {
                sessionId = entity.sessionId,
                isDead = entity.IsDead,
                entityConfigurationResourcePath = $"{EntityConfigurationsBaseResourcePath}/{entity.EntityConfiguration.EntityID}"
            };
        }
    }
}