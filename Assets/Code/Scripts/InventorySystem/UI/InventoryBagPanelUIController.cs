using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.UI;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryBagPanelUIController
    {
        #region UI Elements Names & Classes
        private static readonly string BAG_SLOTS_CONTAINER_LABEL_UI_NAME = "BagSlotsContainer";
        private static readonly string ITEM_ICON_UI_NAME = "ItemIcon";
        private static readonly string ITEM_DETAILS_PANEL_ROOT_UI_NAME = "ItemDetailsContainer";
        private static readonly string ITEM_NAME_LABEL_UI_NAME = "ItemNameLabel";
        private static readonly string ITEM_DESCRIPTION_LABEL_UI_NAME = "ItemDescriptionLabel";
        private static readonly string ITEM_DETAILS_CONTENT_ROOT_UI_NAME = "ItemDetailsContentRoot";
        private static readonly string STYCAS_COUNT_LABEL_UI_NAME = "StycasCountLabel";
        #endregion

        public Action<InventorySlot> onItemSlotSelected;
        public Action<InventorySlot> onItemSlotEquipClicked;

        private readonly VisualElement _root;
        private readonly Dictionary<VisualElement, ItemSlotContainerUIController> _itemSlotContainers = new();
        private readonly VisualElement _itemDetailsContentRoot;
        private readonly VisualElement _itemDetailsIcon;
        private readonly Label _itemDetailsNameLabel;
        private readonly Label _itemDetailsDescriptionLabel;
        private readonly ItemDetailsContentUIController _itemDetailsContentController;

        private Inventory _currentInventory;

        public InventoryBagPanelUIController(
            VisualElement root,
            VisualTreeAsset itemStatContainerTemplate
        )
        {
            // Get UI elements
            _root = root;
            _itemDetailsContentRoot = _root.Q<VisualElement>(ITEM_DETAILS_PANEL_ROOT_UI_NAME);
            _itemDetailsIcon = _root.Q<VisualElement>(ITEM_ICON_UI_NAME);
            _itemDetailsNameLabel = _root.Q<Label>(ITEM_NAME_LABEL_UI_NAME);
            _itemDetailsDescriptionLabel = _root.Q<Label>(ITEM_DESCRIPTION_LABEL_UI_NAME);
            _itemDetailsContentController = new ItemDetailsContentUIController(
                _root.Q<VisualElement>(ITEM_DETAILS_CONTENT_ROOT_UI_NAME),
                itemStatContainerTemplate
            );

            // Setup stycas count
            _root.Q<Label>(STYCAS_COUNT_LABEL_UI_NAME).text = HeroTeam.Instance.Stycas.ToString();

            // Setup item slot containers
            VisualElement bagSlotsContainer = _root.Q<VisualElement>(BAG_SLOTS_CONTAINER_LABEL_UI_NAME);
            foreach (VisualElement child in bagSlotsContainer.Children())
            {
                ItemSlotContainerUIController itemSlotContainer = new(child);
                itemSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
                itemSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotEquipClicked?.Invoke(selectedItem);
                _itemSlotContainers.Add(child, itemSlotContainer);
            }

            ClearItemDetails();
        }

        public void SetInventory(Inventory newInventory)
        {
            _currentInventory = newInventory;
            UpdateBagSlots();
        }

        public void DisplayItemDetails(ItemSO item)
        {
            _itemDetailsContentRoot.visible = true;

            _itemDetailsNameLabel.text = item.Name;
            _itemDetailsDescriptionLabel.text = item.Description;
            _itemDetailsIcon.style.backgroundImage = new(item.IconSprite);

            _itemDetailsContentController.SetItem(item);
        }

        public void ClearItemDetails()
        {
            _itemDetailsContentRoot.visible = false;

            _itemDetailsNameLabel.text = string.Empty;
            _itemDetailsDescriptionLabel.text = string.Empty;
            _itemDetailsIcon.style.backgroundImage = null;

            _itemDetailsContentController.ClearItem();
        }

        private void UpdateBagSlots()
        {
            for (int i = 0; i < _itemSlotContainers.Count; i++)
            {
                ItemSlotContainerUIController itemSlotContainer = _itemSlotContainers.ElementAt(i).Value;
                itemSlotContainer.SetItemSlot(_currentInventory.BagSlots[i]);
            }
        }
    }
}