using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils.UI;
using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    public enum EItemSlotTag
    {
        HEAD,
        CHEST,
        HANDS,
        LEGS,
        FEET,
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
                EItemSlotTag.HANDS => false,
                EItemSlotTag.LEGS => false,
                EItemSlotTag.FEET => false,
                EItemSlotTag.WEAPON => false,
                EItemSlotTag.BAG => true,
                _ => false,
            };
        }

        public static Sprite GetUIIcon(this EItemSlotTag slotTag)
        {
            return slotTag switch
            {
                EItemSlotTag.HEAD => UIIconsProvider.Instance.GetIcon(UIIcons.HELMET.GetIconResourceName()),
                EItemSlotTag.CHEST => UIIconsProvider.Instance.GetIcon(UIIcons.CHESTPLATE.GetIconResourceName()),
                EItemSlotTag.HANDS => UIIconsProvider.Instance.GetIcon(UIIcons.GAUNTLETS.GetIconResourceName()),
                EItemSlotTag.LEGS => UIIconsProvider.Instance.GetIcon(UIIcons.LEGGINGS.GetIconResourceName()),
                EItemSlotTag.FEET => UIIconsProvider.Instance.GetIcon(UIIcons.BOOTS.GetIconResourceName()),
                EItemSlotTag.WEAPON => UIIconsProvider.Instance.GetIcon(UIIcons.WEAPON.GetIconResourceName()),
                EItemSlotTag.BAG => UIIconsProvider.Instance.GetIcon(UIIcons.BAG.GetIconResourceName()),
                _ => null,
            };
        }
    }
}