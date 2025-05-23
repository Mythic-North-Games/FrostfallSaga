using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryBagPanelUIController
    {
        private readonly VisualElement _itemDetailsContainerRoot;
        private readonly ObjectDetailsUIController _itemDetailsController;
        private readonly Dictionary<VisualElement, ItemSlotContainerUIController> _itemSlotContainers = new();

        private readonly VisualElement _root;
        private Inventory _currentInventory;

        public Action<ItemSlotContainerUIController> onItemSlotEquipClicked;
        public Action<ItemSlotContainerUIController> onItemSlotSelected;

        public InventoryBagPanelUIController(
            VisualElement root,
            VisualTreeAsset itemStatContainerTemplate,
            Color statValueColor = default,
            Color statIconColor = default
        )
        {
            // Get UI elements
            _root = root;
            _itemDetailsContainerRoot = _root.Q<VisualElement>(ITEM_DETAILS_CONTAINER_ROOT_UI_NAME);
            _itemDetailsController = new ObjectDetailsUIController(
                _itemDetailsContainerRoot,
                itemStatContainerTemplate,
                ITEM_EFFECT_LINE_CLASSNAME,
                statValueColor,
                statIconColor
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

            HideItemDetails();
        }

        public void SetInventory(Inventory newInventory)
        {
            _currentInventory = newInventory;
            UpdateBagSlots();
        }

        public void HideItems()
        {
            _itemSlotContainers.Values.ToList().ForEach(itemSlotController => itemSlotController.HideItem());
        }

        public void DisplayItemDetails(ItemSO item)
        {
            _itemDetailsContainerRoot.style.display = DisplayStyle.Flex;
            _itemDetailsController.Setup(item);
        }

        public void HideItemDetails()
        {
            _itemDetailsContainerRoot.style.display = DisplayStyle.None;
        }

        private void UpdateBagSlots()
        {
            for (int i = 0; i < _itemSlotContainers.Count; i++)
            {
                ItemSlotContainerUIController itemSlotContainer = _itemSlotContainers.ElementAt(i).Value;
                itemSlotContainer.SetItemSlot(_currentInventory.BagSlots[i]);
            }
        }

        #region UI Elements Names & Classes

        private static readonly string BAG_SLOTS_CONTAINER_LABEL_UI_NAME = "BagSlotsContainer";
        private static readonly string ITEM_DETAILS_CONTAINER_ROOT_UI_NAME = "ItemDetailsContainer";
        private static readonly string STYCAS_COUNT_LABEL_UI_NAME = "StycasCountLabel";

        private static readonly string ITEM_EFFECT_LINE_CLASSNAME = "effectLine";

        #endregion
    }
}