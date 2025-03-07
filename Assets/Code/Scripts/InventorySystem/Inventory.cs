using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostfallSaga.InventorySystem
{
    [Serializable]
    public class Inventory
    {
        public static readonly string DefaultWeaponResourcePath = "ScriptableObjects/Items/ExampleWeapon";

        ///////////////////////
        /// Equipment slots ///
        ///////////////////////
        [field: SerializeField]
        public InventorySlot WeaponSlot { get; protected set; }

        [field: SerializeField] public InventorySlot HelmetSlot { get; protected set; }
        [field: SerializeField] public InventorySlot ChestplateSlot { get; protected set; }
        [field: SerializeField] public InventorySlot GlovesSlot { get; protected set; }
        [field: SerializeField] public InventorySlot LeggingsSlot { get; protected set; }
        [field: SerializeField] public InventorySlot BootsSlot { get; protected set; }

        /////////////////
        /// Bag slots ///
        /////////////////
        [field: SerializeField]
        public InventorySlot[] BagSlots { get; protected set; }

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
            for (int i = 0; i < BagSlots.Length; i++) BagSlots[i] = new InventorySlot();
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

        public AArmor GetGloves()
        {
            return GlovesSlot.Item as AArmor;
        }

        public AArmor GetLeggings()
        {
            return LeggingsSlot.Item as AArmor;
        }

        public AArmor GetBoots()
        {
            return BootsSlot.Item as AArmor;
        }

        public AArmor[] GetArmorPieces()
        {
            List<AArmor> armorPieces = new();
            if (!HelmetSlot.IsEmpty()) armorPieces.Add(HelmetSlot.Item as AArmor);
            if (!ChestplateSlot.IsEmpty()) armorPieces.Add(ChestplateSlot.Item as AArmor);
            if (!GlovesSlot.IsEmpty()) armorPieces.Add(GlovesSlot.Item as AArmor);
            if (!LeggingsSlot.IsEmpty()) armorPieces.Add(LeggingsSlot.Item as AArmor);
            if (!BootsSlot.IsEmpty()) armorPieces.Add(BootsSlot.Item as AArmor);
            return armorPieces.ToArray();
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
                for (int i = 0; i < replacedItem.Item2; i++)
                    AddItemToBag(replacedItem.Item1);

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
                if (slot.Item == item)
                    return slot;

            return null;
        }

        private InventorySlot GetFirstEmptyBagSlot()
        {
            return BagSlots.FirstOrDefault(slot => slot.IsEmpty());
        }

        #endregion
    }
}