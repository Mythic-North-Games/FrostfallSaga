using FrostfallSaga.Core.Fight;
using UnityEngine;

namespace FrostfallSaga.Core.Entities
{
    [CreateAssetMenu(fileName = "EntityConfiguration", menuName = "ScriptableObjects/Entities/EntityConfiguration",
        order = 0)]
    public class EntityConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite DiamondIcon { get; private set; }
        [field: SerializeField] public GameObject KingdomEntityPrefab { get; private set; }
        [field: SerializeField] public GameObject InventoryVisualPrefab { get; private set; }
        [field: SerializeField] public FighterConfigurationSO FighterConfiguration { get; private set; }
        [field: SerializeField] public EEntityRace Race { get; private set; }
        [field: SerializeField, Range(90f, 360f)] public float KingdomDetectionRange { get; private set; } = 90f;
    }
}