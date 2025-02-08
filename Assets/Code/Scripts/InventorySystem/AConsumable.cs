namespace FrostfallSaga.InventorySystem
{
    public abstract class AConsumable : ItemSO
    {
        public AConsumable()
        {
            SlotTag = EItemSlotTag.BAG;
        }
    }
}