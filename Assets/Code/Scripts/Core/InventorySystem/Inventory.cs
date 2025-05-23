using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    [Serializable]
    public class Inventory
    {
        public static readonly string DefaultWeaponResourcePath = "ScriptableObjects/Items/Weapons/ExampleWeapon";

        ///////////////////////
        /// Equipment slots ///
        ///////////////////////
        [field: SerializeField]
        public InventorySlot WeaponSlot { get; protected set; }

        [field: SerializeField] public InventorySlot HelmetSlot { get; protected set; }
        [field: SerializeField] public InventorySlot ChestplateSlot { get; protected set; }
        [field: SerializeField] public InventorySlot GauntletsSlot { get; protected set; }
        [field: SerializeField] public InventorySlot LeggingsSlot { get; protected set; }
        [field: SerializeField] public InventorySlot BootsSlot { get; protected set; }

        /////////////////
        /// Bag slots ///
        /////////////////
        [field: SerializeField]
        public InventorySlot[] BagSlots { get; protected set; }

        [field: SerializeField] public InventorySlot[] ConsumablesQuickAccessSlots { get; protected set; }

        ////////////////////////
        /// Inventory events ///
        ////////////////////////
        public Action<Inventory> OnInventoryUpdated;

        public Inventory()
        {
            WeaponSlot = new InventorySlot(1, EItemSlotTag.WEAPON);
            HelmetSlot = new InventorySlot(1, EItemSlotTag.HEAD);
            ChestplateSlot = new InventorySlot(1, EItemSlotTag.CHEST);
            GauntletsSlot = new InventorySlot(1, EItemSlotTag.HANDS);
            LeggingsSlot = new InventorySlot(1, EItemSlotTag.LEGS);
            BootsSlot = new InventorySlot(1, EItemSlotTag.FEET);

            BagSlots = new InventorySlot[24];
            for (int i = 0; i < BagSlots.Length; i++) BagSlots[i] = new InventorySlot();

            ConsumablesQuickAccessSlots = new InventorySlot[6];
            for (int i = 0; i < ConsumablesQuickAccessSlots.Length; i++)
                ConsumablesQuickAccessSlots[i] = new InventorySlot();
        }

        #region Getters

        /// <summary>
        /// Get the weapon equipped by the player if there is one.
        /// </summary>
        /// <returns>The weapon equipped by the player, or null if there is none</returns>
        public AWeapon GetWeapon()
        {
            return WeaponSlot.Item as AWeapon;
        }

        /// <summary>
        /// Get the helmet equipped by the player if there is one.
        /// </summary>
        /// <returns>The helmet equipped by the player, or null if there is none</returns>
        public AArmor GetHelmet()
        {
            return HelmetSlot.Item as AArmor;
        }

        /// <summary>
        /// Get the chestplate equipped by the player if there is one.
        /// </summary>
        /// <returns>The chestplate equipped by the player, or null if there is none</returns>
        public AArmor GetChestplate()
        {
            return ChestplateSlot.Item as AArmor;
        }

        /// <summary>
        /// Get the gauntlets equipped by the player if there are some.
        /// </summary>
        /// <returns>The gauntlets equipped by the player, or null if there are none</returns>
        public AArmor GetGauntlets()
        {
            return GauntletsSlot.Item as AArmor;
        }

        /// <summary>
        /// Get the leggings equipped by the player if there are some.
        /// </summary>
        /// <returns>The leggings equipped by the player, or null if there are none</returns>
        public AArmor GetLeggings()
        {
            return LeggingsSlot.Item as AArmor;
        }

        /// <summary>
        /// Get the boots equipped by the player if there are some.
        /// </summary>
        /// <returns>The boots equipped by the player, or null if there are none</returns>
        public AArmor GetBoots()
        {
            return BootsSlot.Item as AArmor;
        }

        /// <summary>
        /// Get all the armor pieces equipped by the player.
        /// </summary>
        /// <returns>An array of all the armor pieces equipped by the player</returns>
        public AArmor[] GetArmorPieces()
        {
            List<AArmor> armorPieces = new();
            if (!HelmetSlot.IsEmpty()) armorPieces.Add(HelmetSlot.Item as AArmor);
            if (!ChestplateSlot.IsEmpty()) armorPieces.Add(ChestplateSlot.Item as AArmor);
            if (!GauntletsSlot.IsEmpty()) armorPieces.Add(GauntletsSlot.Item as AArmor);
            if (!LeggingsSlot.IsEmpty()) armorPieces.Add(LeggingsSlot.Item as AArmor);
            if (!BootsSlot.IsEmpty()) armorPieces.Add(BootsSlot.Item as AArmor);
            return armorPieces.ToArray();
        }

        /// <summary>
        /// Get all the equipment slots.
        /// </summary>
        /// <returns>An array of all the equipment slots</returns>
        public InventorySlot[] GetEquipmentSlots()
        {
            return new InventorySlot[] { WeaponSlot, HelmetSlot, ChestplateSlot, GauntletsSlot, LeggingsSlot, BootsSlot };
        }

        /// <summary>
        /// Get the consumable slots in the bag.
        /// </summary>
        /// <returns>An array of all the consumable slots in the bag</returns>
        public InventorySlot[] GetConsumableSlotsInBag()
        {
            return BagSlots.Where(slot => slot.Item is AConsumable).ToArray();
        }

        /// <summary>
        /// Get all the consumables in the bag.
        /// </summary>
        /// <returns>An array of all the consumables in the bag</returns>
        public AConsumable[] GetConsumables()
        {
            return BagSlots
                .Where(slot => slot.Item is AConsumable)
                .Select(slot => slot.Item as AConsumable)
                .ToArray();
        }

        /// <summary>
        /// Get all the items in the inventory.
        /// </summary>
        /// <returns>An array of all the items in the inventory</returns>
        public ItemSO[] GetAllItems()
        {
            List<ItemSO> allItems = new();
            allItems.AddRange(BagSlots.Where(slot => !slot.IsEmpty()).Select(slot => slot.Item));
            allItems.AddRange(ConsumablesQuickAccessSlots.Where(slot => !slot.IsEmpty()).Select(slot => slot.Item));
            allItems.AddRange(GetArmorPieces());
            if (!WeaponSlot.IsEmpty())
            {
                allItems.Add(WeaponSlot.Item);
            }

            return allItems.ToArray();
        }

        #endregion

        #region Inventory management

        /// <summary>
        /// Equip an equipment item (weapon, armor) to the player.
        /// </summary>
        /// <param name="item">The item to equip</param>
        /// <param name="isFromBag">True if the item comes from the bag, false otherwise</param>
        /// <exception cref="InventoryException">Thrown if the item is not equippable.</exception>
        public void EquipEquipment(ItemSO item, bool isFromBag = true)
        {
            // If item comes from the bag, remove it
            if (isFromBag)
            {
                InventorySlot bagSlotWithItem = GetBagSlotWithItem(item);
                bagSlotWithItem?.RemoveItem();
            }

            Tuple<ItemSO, int> replacedItem = item.SlotTag switch
            {
                EItemSlotTag.HEAD => HelmetSlot.ReplaceItem(item, 1),
                EItemSlotTag.CHEST => ChestplateSlot.ReplaceItem(item, 1),
                EItemSlotTag.HANDS => GauntletsSlot.ReplaceItem(item, 1),
                EItemSlotTag.LEGS => LeggingsSlot.ReplaceItem(item, 1),
                EItemSlotTag.FEET => BootsSlot.ReplaceItem(item, 1),
                EItemSlotTag.WEAPON => WeaponSlot.ReplaceItem(item, 1),
                EItemSlotTag.BAG => throw new InventoryException("Cannot equip bag items"),
                _ => null
            };

            // Then add the replaced item that was equipped in the bag if there is one
            if (replacedItem != null)
                for (int i = 0; i < replacedItem.Item2; i++)
                    AddItemToBag(replacedItem.Item1);

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
                EquipEquipment(item, isFromBag: false);
                return true;
            }
            catch (InventoryException)
            {
                AddItemToBag(item);
                return false;
            }
        }

        /// <summary>
        /// Unequip an item from the player and place it the bag.
        /// </summary>
        /// <param name="itemSlot">The item slot to unequip.</param>
        public void UnequipItem(InventorySlot itemSlot)
        {
            for (int i = 0; i < itemSlot.ItemCount; i++)
            {
                AddItemToBag(itemSlot.Item);
            }

            itemSlot.RemoveAllItems();
            OnInventoryUpdated?.Invoke(this);
        }

        /// <summary>
        /// Add an item to the bag. If the item is a consumable, it will be added to the quick access slots if possible.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <exception cref="InventoryException">Thrown if there is no more space in the bag</exception>
        public void AddItemToBag(ItemSO item)
        {
            // Add item to an existing slot if possible
            InventorySlot existingSlotForItem = GetBagSlotWithItem(item);
            if (existingSlotForItem != null && !existingSlotForItem.IsFull())
            {
                existingSlotForItem.AddItem(item);
                return;
            }

            // Otherwise, add item to the first empty slot
            InventorySlot emptySlot = GetFirstEmptyBagSlot();
            if (emptySlot == null) throw new InventoryException("No more space in the bag");
            emptySlot.AddItem(item);
        }

        /// <summary>
        /// Add the given item slot containing a consumable to the quick access slots.
        /// If the quick access slot is full, replace the last item slot and place it in the bag.
        /// </summary>
        /// <param name="consumableSlot">The consumable slot to add</param>
        public void AddConsumableSlotToQuickAccess(InventorySlot consumableSlot)
        {
            // Check if a consumable of the same type is already in the quick access slots
            InventorySlot existingSlotForItem = GetQuickAccessSlotWithItem(consumableSlot.Item);
            if (existingSlotForItem != null)
            {
                for (int i = 0; i < consumableSlot.ItemCount; i++)
                {
                    existingSlotForItem.AddItem(consumableSlot.Item);
                }

                OnInventoryUpdated?.Invoke(this);
                return;
            }

            // Otherwise, add the consumable to the first empty slot if there is one
            InventorySlot emptyQuickAccessSlot = GetFirstEmptyConsumablesQuickAccessSlot();
            if (emptyQuickAccessSlot != null)
            {
                emptyQuickAccessSlot.ReplaceItem(consumableSlot.Item, consumableSlot.ItemCount);
            }

            // If the quick access slots are full, replace the last item slot and place it in the bag
            else
            {
                InventorySlot lastQuickAccessSlot = ConsumablesQuickAccessSlots.Last();
                Tuple<ItemSO, int> replacedItem = lastQuickAccessSlot.ReplaceItem(
                    consumableSlot.Item, consumableSlot.ItemCount
                );
                for (int i = 0; i < replacedItem.Item2; i++)
                {
                    AddItemToBag(replacedItem.Item1);
                }
            }

            consumableSlot.RemoveAllItems();
            OnInventoryUpdated?.Invoke(this);
        }

        /// <summary>
        /// Add a consumable to the quick access slots if possible.
        /// </summary>
        /// <param name="consumable">The consumable to add</param>
        /// <returns>True if the consumable was added to the quick access slots, false otherwise</returns>
        public bool AddConsumableToQuickAccessIfNotFull(AConsumable consumable)
        {
            // Try to add the consumable to an existing slot if possible
            InventorySlot existingSlotForItem = GetQuickAccessSlotWithItem(consumable);
            if (existingSlotForItem != null && !existingSlotForItem.IsFull())
            {
                existingSlotForItem.AddItem(consumable);
                return true;
            }

            // Otherwise, add the consumable to the first empty slot if there is one
            InventorySlot emptyQuickAccessSlot = GetFirstEmptyConsumablesQuickAccessSlot();
            if (emptyQuickAccessSlot == null) return false;
            emptyQuickAccessSlot.AddItem(consumable);
            return true;

        }

        /// <summary>
        /// Check if an item can be added to the bag.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item can be added to the bag, false otherwise</returns>
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
            return GetEquipmentSlots().FirstOrDefault(slot => slot.Item == item);
        }

        private InventorySlot GetBagSlotWithItem(ItemSO item)
        {
            return BagSlots.FirstOrDefault(slot => slot.Item == item);
        }

        private InventorySlot GetQuickAccessSlotWithItem(ItemSO item)
        {
            return ConsumablesQuickAccessSlots.FirstOrDefault(slot => slot.Item == item);
        }

        private InventorySlot GetFirstEmptyBagSlot()
        {
            return BagSlots.FirstOrDefault(slot => slot.IsEmpty());
        }

        private InventorySlot GetFirstEmptyConsumablesQuickAccessSlot()
        {
            return ConsumablesQuickAccessSlots.FirstOrDefault(slot => slot.IsEmpty());
        }

        #endregion
    }
}