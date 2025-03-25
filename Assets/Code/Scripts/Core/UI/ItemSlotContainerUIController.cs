using System;
using UnityEngine.UIElements;
using FrostfallSaga.Core.InventorySystem;

namespace FrostfallSaga.Core.UI
{
    public class ItemSlotContainerUIController
    {
        #region UI Elements Names & Classes
        private static readonly string ITEM_ICON_UI_NAME = "ItemSlot";
        private static readonly string ITEM_SLOT_COUNT_CONTAINER_UI_NAME = "ItemSlotCountContainer";
        private static readonly string ITEM_COUNT_LABEL_UI_NAME = "ItemSlotCount";
        #endregion

        public Action<InventorySlot> onItemSelected;
        public Action<InventorySlot> onItemEquipToggled;

        private readonly VisualElement _root;
        private readonly VisualElement _itemIcon;
        private readonly VisualElement _itemCountLabelContainer;
        private readonly Label _itemCountLabel;
        private InventorySlot _currentItemSlot;

        public ItemSlotContainerUIController(VisualElement root)
        {
            _root = root;
            _root.RegisterCallback<MouseUpEvent>(OnItemSelected);
            _itemIcon = _root.Q<VisualElement>(ITEM_ICON_UI_NAME);
            _itemCountLabelContainer = _root.Q<VisualElement>(ITEM_SLOT_COUNT_CONTAINER_UI_NAME);
            _itemCountLabel = root.Q<Label>(ITEM_COUNT_LABEL_UI_NAME);
        }

        /// <summary>
        /// Set the item slot to display in the UI.
        /// </summary>
        /// <param name="itemSlot">The item slot to display.</param>
        public void SetItemSlot(InventorySlot itemSlot)
        {
            _itemIcon.style.backgroundImage = itemSlot.Item == null ? null : new(itemSlot.Item.IconSprite);
            _itemCountLabelContainer.style.display = itemSlot.MaxItemCount > 1 && itemSlot.ItemCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _itemCountLabel.text = itemSlot.MaxItemCount > 1 ? itemSlot.ItemCount.ToString() : string.Empty;

            _currentItemSlot = itemSlot;
        }

        /// <summary>
        /// Set the enabled state of the item slot.
        /// </summary>
        /// <param name="enabled">The enabled state to set.</param>
        public void SetEnabled(bool enabled)
        {
            _root.SetEnabled(enabled);
        }

        private void OnItemSelected(MouseUpEvent clickEvent)
        {
            clickEvent.StopPropagation();
            if (clickEvent.button == 0) onItemSelected?.Invoke(_currentItemSlot);
            else if (clickEvent.button == 1) onItemEquipToggled?.Invoke(_currentItemSlot);
        }
    }
}