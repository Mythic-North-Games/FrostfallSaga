namespace FrostfallSaga.Core.InventorySystem
{
    public enum EItemSlotTag
    {
        HEAD,
        CHEST,
        FEET,
        HANDS,
        WEAPON,
        BAG,
    }

    public static class EItemSlotTagMethods
    {
        public static bool IsStackable(this EItemSlotTag slotTag)
        {
            return slotTag switch
            {
                EItemSlotTag.HEAD => false,
                EItemSlotTag.CHEST => false,
                EItemSlotTag.FEET => false,
                EItemSlotTag.HANDS => false,
                EItemSlotTag.WEAPON => false,
                EItemSlotTag.BAG => true,
                _ => false,
            };
        }
    }
}