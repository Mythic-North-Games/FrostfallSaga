using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    [Serializable]
    public class Inventory
    {
        public static readonly string DefaultWeaponResourcePath = "ScriptableObjects/Items/ExampleWeapon";

        ///////////////////////
        /// Equipment slots ///
        ///////////////////////
        [field: SerializeField] public InventorySlot WeaponSlot { get; protected set; }
        [field: SerializeField] public InventorySlot HelmetSlot { get; protected set; }
        [field: SerializeField] public InventorySlot ChestplateSlot { get; protected set; }
        [field: SerializeField] public InventorySlot GauntletsSlot { get; protected set; }
        [field: SerializeField] public InventorySlot BootsSlot { get; protected set; }

        /////////////////
        /// Bag slots ///
        /////////////////
        [field: SerializeField] public InventorySlot[] BagSlots { get; protected set; }

        ////////////////////////
        /// Inventory events ///
        ////////////////////////
        public Action<Inventory> OnInventoryUpdated;

        public Inventory()
        {
            WeaponSlot = new InventorySlot(1);
            HelmetSlot = new InventorySlot(1);
            ChestplateSlot = new InventorySlot(1);
            GauntletsSlot = new InventorySlot(1);
            BootsSlot = new InventorySlot(1);

            BagSlots = new InventorySlot[35];
            for (int i = 0; i < BagSlots.Length; i++)
            {
                BagSlots[i] = new InventorySlot();
            }
        }

        #region Getters

        public AWeapon GetWeapon()
        {
            return WeaponSlot.Item as AWeapon;
        }

        public AArmor GetHelmet()
        {
            return HelmetSlot.Item as AArmor;
        }

        public AArmor GetChestplate()
        {
            return ChestplateSlot.Item as AArmor;
        }

        public AArmor GetGauntlets()
        {
            return GauntletsSlot.Item as AArmor;
        }

        public AArmor GetBoots()
        {
            return BootsSlot.Item as AArmor;
        }

        public AArmor[] GetArmorPieces()
        {
            List<AArmor> armorPieces = new();
            if (!HelmetSlot.IsEmpty())
            {
                armorPieces.Add(HelmetSlot.Item as AArmor);
            }
            if (!ChestplateSlot.IsEmpty())
            {
                armorPieces.Add(ChestplateSlot.Item as AArmor);
            }
            if (!GauntletsSlot.IsEmpty())
            {
                armorPieces.Add(GauntletsSlot.Item as AArmor);
            }
            if (!BootsSlot.IsEmpty())
            {
                armorPieces.Add(BootsSlot.Item as AArmor);
            }
            return armorPieces.ToArray();
        }

        public AConsumable[] GetConsumables()
        {
            return BagSlots
                .Where(slot => slot.Item is AConsumable)
                .Select(slot => slot.Item as AConsumable)
                .ToArray();
        }

        public ItemSO[] GetAllItems()
        {
            List<ItemSO> allItems = new();
            allItems.AddRange(BagSlots.Where(slot => !slot.IsEmpty()).Select(slot => slot.Item));
            allItems.AddRange(GetArmorPieces());
            if (!WeaponSlot.IsEmpty())
            {
                allItems.Add(WeaponSlot.Item);
            }
            return allItems.ToArray();
        }

        #endregion

        #region Inventory management

        public void EquipItem(ItemSO item, bool fromBag = true)
        {
            // If item comes from the bag, remove it
            if (fromBag)
            {
                InventorySlot bagSlotWithItem = GetBagSlotWithItem(item);
                bagSlotWithItem?.RemoveItem();
            }

            Tuple<ItemSO, int> replacedItem = null;
            switch (item.SlotTag)
            {
                case EItemSlotTag.WEAPON:
                    replacedItem = WeaponSlot.ReplaceItem(item, 1);
                    break;
                case EItemSlotTag.HEAD:
                    replacedItem = HelmetSlot.ReplaceItem(item, 1);
                    break;
                case EItemSlotTag.CHEST:
                    replacedItem = ChestplateSlot.ReplaceItem(item, 1);
                    break;
                case EItemSlotTag.HANDS:
                    replacedItem = GauntletsSlot.ReplaceItem(item, 1);
                    break;
                case EItemSlotTag.FEET:
                    replacedItem = BootsSlot.ReplaceItem(item, 1);
                    break;
                case EItemSlotTag.BAG:
                    throw new InventoryException("Cannot equip bag items");
            }

            // Then add the replaced item that was equipped in the bag if there is one
            if (replacedItem != null)
            {
                for (int i = 0; i < replacedItem.Item2; i++)
                {
                    AddItemToBag(replacedItem.Item1);
                }
            }
            OnInventoryUpdated?.Invoke(this);
        }

        /// <summary>
        /// Add an item to the inventory and equip it if possible.
        /// If the item is not equippable, it will be added to the bag.
        /// </summary>
        /// <param name="item">The item to add and try to equip</param>
        /// <returns>True if the item was equipped, false if it was added to the bag</returns>
        public bool AddItemAndEquipIfPossible(ItemSO item)
        {
            try
            {
                EquipItem(item, fromBag: false);
                return true;
            }
            catch (InventoryException)
            {
                AddItemToBag(item);
                return false;
            }
        }

        public void UnequipItem(ItemSO item)
        {
            InventorySlot slot = GetEquipmentSlotWithItem(item);
            if (slot == null || slot.IsEmpty())
            {
                throw new InventoryException("Item not equipped");
            }
            
            AddItemToBag(slot.Item);
            slot.RemoveItem();
            OnInventoryUpdated?.Invoke(this);
        }

        public void AddItemToBag(ItemSO item)
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

        public bool CanAddItemToBag(ItemSO item)
        {
            InventorySlot existingSlotForItem = GetBagSlotWithItem(item);
            if (existingSlotForItem != null && !existingSlotForItem.IsFull())
            {
                return true;
            }
            InventorySlot emptySlot = GetFirstEmptyBagSlot();
            return emptySlot != null;
        }

        private InventorySlot GetEquipmentSlotWithItem(ItemSO item)
        {
            if (WeaponSlot.Item == item)
            {
                return WeaponSlot;
            }
            if (HelmetSlot.Item == item)
            {
                return HelmetSlot;
            }
            if (ChestplateSlot.Item == item)
            {
                return ChestplateSlot;
            }
            if (GauntletsSlot.Item == item)
            {
                return GauntletsSlot;
            }
            if (BootsSlot.Item == item)
            {
                return BootsSlot;
            }
            return null;
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

        private bool ItemIsEquipment(ItemSO item)
        {
            return item.SlotTag != EItemSlotTag.BAG;
        }

        #endregion
    }
}