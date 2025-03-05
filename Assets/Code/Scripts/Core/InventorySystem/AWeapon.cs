namespace FrostfallSaga.Core.InventorySystem
{
    public abstract class AWeapon : AEquipment
    {
        public AWeapon()
        {
            SlotTag = EItemSlotTag.WEAPON;
        }
    }
}