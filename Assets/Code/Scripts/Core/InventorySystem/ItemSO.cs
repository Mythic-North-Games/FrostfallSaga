using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Item", order = 0)]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite IconSprite { get; protected set; }
        [field: SerializeField] public EItemSlotTag SlotTag { get; protected set; }
        [field: SerializeField] public float LootChance { get; protected set; }
    }
}