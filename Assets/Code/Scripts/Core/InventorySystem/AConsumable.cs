using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    public abstract class AConsumable : ItemSO
    {
        [field: SerializeField] public int ActionPointsCost { get; private set; }

        public AConsumable()
        {
            SlotTag = EItemSlotTag.BAG;
        }

        public override string GetPrimaryEffectsTitle()
        {
            return "Effects";
        }
    }
}