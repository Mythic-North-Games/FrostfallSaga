using UnityEngine;

namespace FrostfallSaga.Kingdom.Entities
{

    [CreateAssetMenu(fileName = "EntityConfiguration", menuName = "ScriptableObjects/Entities/EntityConfiguration", order = 0)]
    public class EntityConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public EntityType EntityType { get; private set; }
        [field: SerializeField] public Sprite EntityIcon { get; private set; }
    }
}