using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.UI;
using UnityEngine;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryBagPanelUIController
    {
        #region UI Elements Names & Classes
        private static readonly string BAG_SLOTS_CONTAINER_LABEL_UI_NAME = "BagSlotsContainer";
        private static readonly string ITEM_DETAILS_CONTAINER_ROOT_UI_NAME = "ItemDetailsContainer";
        private static readonly string STYCAS_COUNT_LABEL_UI_NAME = "StycasCountLabel";

        private static readonly string ITEM_EFFECT_LINE_CLASSNAME = "effectLine";
        #endregion

        public Action<InventorySlot> onItemSlotSelected;
        public Action<InventorySlot> onItemSlotEquipClicked;

        private readonly VisualElement _root;
        private readonly Dictionary<VisualElement, ItemSlotContainerUIController> _itemSlotContainers = new();
        private readonly VisualElement _itemDetailsContainerRoot;
        private readonly ObjectDetailsUIController _itemDetailsController;

        private Inventory _currentInventory;

        public InventoryBagPanelUIController(
            VisualElement root,
            VisualTreeAsset itemStatContainerTemplate,
            Color statValueColor = default
        )
        {
            // Get UI elements
            _root = root;
            _itemDetailsContainerRoot = _root.Q<VisualElement>(ITEM_DETAILS_CONTAINER_ROOT_UI_NAME);
            _itemDetailsController = new ObjectDetailsUIController(
                _itemDetailsContainerRoot,
                itemStatContainerTemplate,
                ITEM_EFFECT_LINE_CLASSNAME,
                statValueColor
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

        public void DisplayItemDetails(ItemSO item)
        {
            _itemDetailsContainerRoot.style.display = DisplayStyle.Flex;
            _itemDetailsController.Setup(
                name: item.Name,
                description: item.Description,
                icon: item.IconSprite,
                stats: GetItemStats(item),
                primaryEffectsTitle: item is AEquipment ? "Special effects" : "Effects",
                primaryEffects: GetItemEffects(item)
            );
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

        private Dictionary<Sprite, string> GetItemStats(ItemSO item)
        {
            Dictionary<Sprite, string> itemStats = new();
            if (item is AEquipment equipment)
            {
                itemStats.Concat(equipment.GetStatsUIData()).Concat(equipment.GetMagicalStatsUIData());
            }
            return itemStats;
        }

        private List<string> GetItemEffects(ItemSO item)
        {
            if (item is AEquipment equipment) return equipment.GetSpecialEffectsUIData();
            if (item is AConsumable consumable) return consumable.GetEffectsUIData();
            return new List<string>();
        }
    }
}