using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.HeroTeam;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryBagPanelUIController
    {
        #region UI Elements Names & Classes
        private static readonly string BAG_SLOTS_CONTAINER_LABEL_UI_NAME = "BagSlotsContainer";
        private static readonly string ITEM_ICON_UI_NAME = "ItemIcon";
        private static readonly string ITEM_NAME_LABEL_UI_NAME = "ItemNameLabel";
        private static readonly string ITEM_DESCRIPTION_LABEL_UI_NAME = "ItemDescriptionLabel";
        private static readonly string ITEM_DETAILS_CONTENT_ROOT_UI_NAME = "ItemDetailsContentRoot";
        private static readonly string STYCAS_COUNT_LABEL_UI_NAME = "StycasCountLabel";
        #endregion

        public Action<InventorySlot> onItemSlotSelected;
        public Action<InventorySlot> onItemSlotEquipClicked;

        private readonly VisualElement _root;
        private readonly Dictionary<VisualElement, ItemSlotContainerUIController> _itemSlotContainers = new();
        private readonly ItemDetailsContentUIController _itemDetailsContentController;

        private Inventory _currentInventory;

        public InventoryBagPanelUIController(
            VisualElement root,
            VisualTreeAsset itemStatContainerTemplate
        )
        {
            _root = root;
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
            _root.Q<Label>(ITEM_NAME_LABEL_UI_NAME).text = item.Name;
            _root.Q<Label>(ITEM_DESCRIPTION_LABEL_UI_NAME).text = item.Description;
            _root.Q<VisualElement>(ITEM_ICON_UI_NAME).style.backgroundImage = new(item.IconSprite);
            _itemDetailsContentController.SetItem(item);
        }

        public void ClearItemDetails()
        {
            _root.Q<Label>(ITEM_NAME_LABEL_UI_NAME).text = string.Empty;
            _root.Q<Label>(ITEM_DESCRIPTION_LABEL_UI_NAME).text = string.Empty;
            _root.Q<VisualElement>(ITEM_ICON_UI_NAME).style.backgroundImage = null;
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