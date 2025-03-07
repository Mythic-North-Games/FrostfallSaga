using System;
using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        [field: SerializeField] public ItemSO Item { get; private set; }
        [field: SerializeField] public int ItemCount { get; private set; }
        [field: SerializeField] public int MaxItemCount { get; private set; }

        public InventorySlot()
        {
            Item = null;
            ItemCount = 0;
            MaxItemCount = 99;
        }

        public InventorySlot(int maxItemCount)
        {
            Item = null;
            ItemCount = 0;
            MaxItemCount = maxItemCount;
        }

        public Tuple<ItemSO, int> ReplaceItem(ItemSO item, int itemCount)
        {
            Tuple<ItemSO, int> replacedItem = new(Item, ItemCount);
            Item = item;
            ItemCount = itemCount;
            MaxItemCount = item.SlotTag.IsStackable() ? 99 : 1;
            return replacedItem;
        }

        public void AddItem(ItemSO newItem)
        {
            if (newItem != Item)
            {
                ReplaceItem(newItem, 1);
                return;
            }

            if (ItemCount >= MaxItemCount) throw new InventoryException("Inventory slot is full");

            ItemCount++;
        }

        public void RemoveItem()
        {
            if (ItemCount > 1)
            {
                ItemCount--;
            }
            else
            {
                Item = null;
                ItemCount = 0;
            }
        }

        public bool IsEmpty()
        {
            return Item == null;
        }

        public bool IsFull()
        {
            return ItemCount == MaxItemCount;
        }
    }
}