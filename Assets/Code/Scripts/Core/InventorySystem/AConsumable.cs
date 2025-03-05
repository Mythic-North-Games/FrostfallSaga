using System.Collections.Generic;

namespace FrostfallSaga.Core.InventorySystem
{
    public abstract class AConsumable : ItemSO
    {
        public AConsumable()
        {
            SlotTag = EItemSlotTag.BAG;
        }

        public abstract List<string> GetSpecialEffectsUIData();
    }
}