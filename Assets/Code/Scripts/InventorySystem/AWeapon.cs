namespace FrostfallSaga.InventorySystem
{
    public abstract class AWeapon : ItemSO
    {
        public AWeapon()
        {
            SlotTag = EItemSlotTag.WEAPON;
        }
    }
}