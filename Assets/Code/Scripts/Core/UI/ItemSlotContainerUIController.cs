using System;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class ItemSlotContainerUIController
    {
        private readonly Label _itemCountLabel;
        private readonly VisualElement _itemCountLabelContainer;
        private readonly VisualElement _itemIcon;
        private InventorySlot _currentItemSlot;
        public Action<InventorySlot> onItemEquipToggled;

        public Action<InventorySlot> onItemSelected;

        public ItemSlotContainerUIController(VisualElement root)
        {
            Root = root;
            Root.RegisterCallback<MouseUpEvent>(OnItemSelected);
            _itemIcon = Root.Q<VisualElement>(ITEM_ICON_UI_NAME);
            _itemCountLabelContainer = Root.Q<VisualElement>(ITEM_SLOT_COUNT_CONTAINER_UI_NAME);
            _itemCountLabel = root.Q<Label>(ITEM_COUNT_LABEL_UI_NAME);
        }

        public VisualElement Root { get; private set; }

        /// <summary>
        /// Set the item slot to display in the UI.
        /// </summary>
        /// <param name="itemSlot">The item slot to display.</param>
        public void SetItemSlot(InventorySlot itemSlot)
        {
            bool itemIsNull = itemSlot == null || itemSlot.Item == null;
            bool itemCountDisplayable = !itemIsNull && itemSlot.MaxItemCount > 1 && itemSlot.ItemCount > 0;

            _itemIcon.style.backgroundImage = itemIsNull ? null : new(itemSlot.Item.IconSprite);
            _itemCountLabelContainer.style.display = itemCountDisplayable ? DisplayStyle.Flex : DisplayStyle.None;
            _itemCountLabel.text = itemCountDisplayable ? itemSlot.ItemCount.ToString() : string.Empty;

            _currentItemSlot = itemSlot;
        }

        /// <summary>
        /// Set the enabled state of the item slot.
        /// </summary>
        /// <param name="enabled">The enabled state to set.</param>
        public void SetEnabled(bool enabled)
        {
            Root.SetEnabled(enabled);
            Root.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
            Root.Children().ToList()
                .ForEach(child => child.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore);
        }

        private void OnItemSelected(MouseUpEvent clickEvent)
        {
            clickEvent.StopPropagation();
            if (clickEvent.button == 0) onItemSelected?.Invoke(_currentItemSlot);
            else if (clickEvent.button == 1) onItemEquipToggled?.Invoke(_currentItemSlot);
        }

        #region UI Elements Names & Classes

        private static readonly string ITEM_ICON_UI_NAME = "ItemSlot";
        private static readonly string ITEM_SLOT_COUNT_CONTAINER_UI_NAME = "ItemSlotCountContainer";
        private static readonly string ITEM_COUNT_LABEL_UI_NAME = "ItemSlotCount";

        #endregion
    }
}