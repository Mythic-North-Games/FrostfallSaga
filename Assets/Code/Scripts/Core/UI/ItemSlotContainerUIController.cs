using System;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class ItemSlotContainerUIController
    {
        #region UI Elements Names & Classes
        private static readonly string ITEM_SLOT_CONTAINER_UI_NAME = "ItemSlotContainer";
        private static readonly string ITEM_ICON_UI_NAME = "ItemSlot";
        private static readonly string ITEM_SLOT_COUNT_CONTAINER_UI_NAME = "ItemSlotCountContainer";
        private static readonly string ITEM_COUNT_LABEL_UI_NAME = "ItemSlotCount";
        private static readonly string ITEM_SLOT_CONTAINER_SELECTED_UI_NAME = "ItemSlotContainerSelected";
        private static readonly string ITEM_SLOT_TAG_ICON_UI_NAME = "ItemSlotTagIcon";

        private static readonly string ITEM_ICON_HIDDEN_CLASSNAME = "itemSlotHidden";
        private static readonly string ITEM_SLOT_COUNT_HIDDEN_CLASSNAME = "itemSlotCountHidden";
        private static readonly string ITEM_SLOT_ACTIVE_CLASSNAME = "itemSlotContainerActive";
        #endregion

        private readonly VisualElement _itemSlotContainer;
        private readonly VisualElement _itemSlotTagIcon;
        private readonly VisualElement _itemIcon;
        private readonly VisualElement _itemSlotCountContainer;
        private readonly Label _itemCountLabel;
        private readonly VisualElement _itemSlotContainerSelected;

        public VisualElement Root { get; private set; }
        public InventorySlot CurrentItemSlot { get; private set; }

        public Action<ItemSlotContainerUIController> onItemEquipToggled;
        public Action<ItemSlotContainerUIController> onItemSelected;

        public ItemSlotContainerUIController(VisualElement root)
        {
            Root = root;
            Root.RegisterCallback<MouseUpEvent>(OnItemSelected);
            _itemSlotContainer = root.Q<VisualElement>(ITEM_SLOT_CONTAINER_UI_NAME);
            _itemSlotTagIcon = root.Q<VisualElement>(ITEM_SLOT_TAG_ICON_UI_NAME);
            _itemIcon = Root.Q<VisualElement>(ITEM_ICON_UI_NAME);
            _itemSlotCountContainer = root.Q<VisualElement>(ITEM_SLOT_COUNT_CONTAINER_UI_NAME);
            _itemCountLabel = root.Q<Label>(ITEM_COUNT_LABEL_UI_NAME);
            _itemSlotContainerSelected = root.Q<VisualElement>(ITEM_SLOT_CONTAINER_SELECTED_UI_NAME);
        }

        /// <summary>
        /// Set the item slot to display in the UI.
        /// </summary>
        /// <param name="itemSlot">The item slot to display.</param>
        public void SetItemSlot(InventorySlot itemSlot)
        {
            bool itemIsNull = itemSlot == null || itemSlot.Item == null;
            bool itemCountDisplayable = !itemIsNull && itemSlot.MaxItemCount > 1 && itemSlot.ItemCount > 0;

            Root.SetEnabled(!itemIsNull);

            if (_itemSlotTagIcon != null) // INFO: Is null for consumables during fight
            {
                _itemSlotTagIcon.style.backgroundImage = itemIsNull ? null : new(itemSlot.SlotTag.GetUIIcon());
                _itemSlotTagIcon.style.opacity = itemIsNull ? 1f : 0.33f;
            }
            if (_itemSlotContainerSelected != null) // INFO: Is null for consumables during fight
            {
                _itemSlotContainerSelected.style.display = itemIsNull ? DisplayStyle.None : DisplayStyle.Flex;
            }

            _itemIcon.style.backgroundImage = itemIsNull ? null : new(itemSlot.Item.IconSprite);

            if (_itemSlotCountContainer != null)
            {
                _itemSlotCountContainer.style.display = itemIsNull ? DisplayStyle.None : DisplayStyle.Flex;
            }

            _itemCountLabel.text = itemCountDisplayable ? itemSlot.ItemCount.ToString() : string.Empty;

            CurrentItemSlot = itemSlot;

            if (!itemIsNull)
            {
                _itemIcon.RemoveFromClassList(ITEM_ICON_HIDDEN_CLASSNAME);
                _itemCountLabel.RemoveFromClassList(ITEM_SLOT_COUNT_HIDDEN_CLASSNAME);
            }
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

        public void SetActive(bool active)
        {
            _itemSlotContainer.EnableInClassList(ITEM_SLOT_ACTIVE_CLASSNAME, active);
        }

        public void HideItem()
        {
            if (CurrentItemSlot == null) return;

            _itemIcon.AddToClassList(ITEM_ICON_HIDDEN_CLASSNAME);
            _itemCountLabel.AddToClassList(ITEM_SLOT_COUNT_HIDDEN_CLASSNAME);
        }

        public void PlayCannotEquipAnimation(UIDocument uiDoc)
        {
            uiDoc.StartCoroutine(CommonUIAnimations.PlayShakeAnimation(_itemIcon));
        }

        private void OnItemSelected(MouseUpEvent clickEvent)
        {
            clickEvent.StopPropagation();
            if (clickEvent.button == (int)MouseButton.LeftMouse) onItemSelected?.Invoke(this);
            else if (clickEvent.button == (int)MouseButton.RightMouse) onItemEquipToggled?.Invoke(this);
        }
    }
}