using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostfallSaga.Fight.GameItems
{
    [Serializable]
    public class Inventory
    {
        public static readonly string DefaultWeaponResourcePath = "ScriptableObjects/Items/ExampleWeapon";

        ///////////////////////
        /// Equipment slots ///
        ///////////////////////
        [field: SerializeField] public InventorySlot WeaponSlot { get; private set; }
        [field: SerializeField] public InventorySlot HelmetSlot { get; private set; }
        [field: SerializeField] public InventorySlot ChestplateSlot { get; private set; }
        [field: SerializeField] public InventorySlot GlovesSlot { get; private set; }
        [field: SerializeField] public InventorySlot LeggingsSlot { get; private set; }
        [field: SerializeField] public InventorySlot BootsSlot { get; private set; }

        /////////////////
        /// Bag slots ///
        /////////////////
        [field: SerializeField] public InventorySlot[] BagSlots { get; private set; }

        ////////////////////////
        /// Inventory events ///
        ////////////////////////
        public Action<Inventory> OnInventoryUpdated;

        public Inventory()
        {
            WeaponSlot = new InventorySlot(1);
            HelmetSlot = new InventorySlot(1);
            ChestplateSlot = new InventorySlot(1);
            GlovesSlot = new InventorySlot(1);
            LeggingsSlot = new InventorySlot(1);
            BootsSlot = new InventorySlot(1);

            BagSlots = new InventorySlot[24];
            for (int i = 0; i < BagSlots.Length; i++)
            {
                BagSlots[i] = new InventorySlot();
            }
        }

        #region Getters

        public WeaponSO GetWeapon()
        {
            return WeaponSlot.Item as WeaponSO;
        }

        public ArmorSO GetHelmet()
        {
            return HelmetSlot.Item as ArmorSO;
        }

        public ArmorSO GetChestplate()
        {
            return ChestplateSlot.Item as ArmorSO;
        }

        public ArmorSO GetGloves()
        {
            return GlovesSlot.Item as ArmorSO;
        }

        public ArmorSO GetLeggings()
        {
            return LeggingsSlot.Item as ArmorSO;
        }

        public ArmorSO GetBoots()
        {
            return BootsSlot.Item as ArmorSO;
        }

        public ArmorSO[] GetArmorPieces()
        {
            List<ArmorSO> armorPieces = new();
            if (!HelmetSlot.IsEmpty())
            {
                armorPieces.Add(HelmetSlot.Item as ArmorSO);
            }
            if (!ChestplateSlot.IsEmpty())
            {
                armorPieces.Add(ChestplateSlot.Item as ArmorSO);
            }
            if (!GlovesSlot.IsEmpty())
            {
                armorPieces.Add(GlovesSlot.Item as ArmorSO);
            }
            if (!LeggingsSlot.IsEmpty())
            {
                armorPieces.Add(LeggingsSlot.Item as ArmorSO);
            }
            if (!BootsSlot.IsEmpty())
            {
                armorPieces.Add(BootsSlot.Item as ArmorSO);
            }
            return armorPieces.ToArray();
        }

        public ConsumableSO[] GetConsumables()
        {
            return BagSlots
                .Where(slot => slot.Item is ConsumableSO)
                .Select(slot => slot.Item as ConsumableSO)
                .ToArray();
        }

        #endregion

        #region Inventory management

        public void AddItem(ItemSO item)
        {
            Tuple<ItemSO, int> replacedItem = null;
            switch (item.SlotTag)
            {
                case EItemSlotTag.WEAPON:
                    replacedItem = WeaponSlot.ReplaceItem(item);
                    break;
                case EItemSlotTag.HEAD:
                    replacedItem = HelmetSlot.ReplaceItem(item);
                    break;
                case EItemSlotTag.CHEST:
                    replacedItem = ChestplateSlot.ReplaceItem(item);
                    break;
                case EItemSlotTag.HANDS:
                    replacedItem = GlovesSlot.ReplaceItem(item);
                    break;
                case EItemSlotTag.LEGS:
                    replacedItem = LeggingsSlot.ReplaceItem(item);
                    break;
                case EItemSlotTag.FEET:
                    replacedItem = BootsSlot.ReplaceItem(item);
                    break;
                case EItemSlotTag.BAG:
                    AddItemToBag(item);
                    break;
            }

            if (replacedItem != null)
            {
                for (int i = 0; i < replacedItem.Item2; i++)
                {
                    AddItemToBag(replacedItem.Item1);
                }
            }
            OnInventoryUpdated?.Invoke(this);
        }

        private void AddItemToBag(ItemSO item)
        {
            InventorySlot existingSlotForItem = GetBagSlotWithItem(item);
            if (existingSlotForItem != null && !existingSlotForItem.IsFull())
            {
                existingSlotForItem.AddItem(item);
                return;
            }
            InventorySlot emptySlot = GetFirstEmptyBagSlot();
            if (emptySlot != null)
            {
                emptySlot.AddItem(item);
                return;
            }
            throw new InventoryException("No more space in the bag");
        }

        private InventorySlot GetBagSlotWithItem(ItemSO item)
        {
            foreach (InventorySlot slot in BagSlots)
            {
                if (slot.Item == item)
                {
                    return slot;
                }
            }
            return null;
        }

        private InventorySlot GetFirstEmptyBagSlot()
        {
            return BagSlots.FirstOrDefault(slot => slot.IsEmpty());
        }

        #endregion
    }
}