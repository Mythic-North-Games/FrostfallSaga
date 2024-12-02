using UnityEngine;
using FrostfallSaga.Core;

namespace FrostfallSaga.Kingdom.Entities
{

    [CreateAssetMenu(fileName = "EntityConfiguration", menuName = "ScriptableObjects/Entities/EntityConfiguration", order = 0)]
    public class EntityConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public EntityID EntityID { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite DiamondIcon { get; private set; }
        [field: SerializeField] public GameObject EntityPrefab { get; private set; }
    }
}