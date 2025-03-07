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

        public InventorySlot[] GetEquipmentSlots()
        {
            return new InventorySlot[] { WeaponSlot, HelmetSlot, ChestplateSlot, GauntletsSlot, BootsSlot };
        }

        public AConsumable[] GetConsumables()
        {
            return BagSlots
                .Where(slot => slot.Item is AConsumable)
                .Select(slot => slot.Item as AConsumable)
                .ToArray();
        }

        #endregion

        #region Inventory management

        public void EquipItem(ItemSO item)
        {
            // If item already in the bag, remove it
            InventorySlot bagSlotWithItem = GetBagSlotWithItem(item);
            bagSlotWithItem?.RemoveItem();

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

            if (replacedItem != null)
            {
                for (int i = 0; i < replacedItem.Item2; i++)
                {
                    AddItemToBag(replacedItem.Item1);
                }
            }
            OnInventoryUpdated?.Invoke(this);
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

        private InventorySlot GetEquipmentSlotWithItem(ItemSO item)
        {
            foreach (InventorySlot slot in GetEquipmentSlots())
            {
                if (slot.Item == item)
                {
                    return slot;
                }
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

        #endregion
    }
}