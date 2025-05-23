using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.Rewards;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class ItemRewardsListUIController
    {
        private readonly VisualElement _root;
        private readonly VisualTreeAsset _itemSlotTemplate;
        private readonly Dictionary<VisualElement, ItemSO> _itemRewardContainerToItem = new();
        private readonly string _itemSlotRootClassname;
        private readonly ObjectDetailsOverlayUIController _objectDetailsOverlayController;

        public ItemRewardsListUIController(
            VisualElement root,
            VisualTreeAsset itemSlotTemplate,
            string itemSlotRootClassname,
            VisualTreeAsset rewardItemDetailsOverlay,
            VisualTreeAsset statContainerTemplate
        )
        {
            _root = root;
            _itemSlotTemplate = itemSlotTemplate;
            _itemSlotRootClassname = itemSlotRootClassname;
            _objectDetailsOverlayController = new(rewardItemDetailsOverlay, statContainerTemplate);
        }

        public void UpdateItemRewardsList(Reward rewardResult)
        {
            _root.Clear();

            if (rewardResult.StycasEarned > 0)
            {
                ItemSO stycasItem = ItemSO.LoadFromResources("Stycas");
                VisualElement stycasRewardContainer = GenerateRewardItemSlot(stycasItem, rewardResult.StycasEarned);
                _itemRewardContainerToItem[stycasRewardContainer] = stycasItem;
                _root.Add(stycasRewardContainer);
            }

            foreach (KeyValuePair<ItemSO, int> rewardItem in rewardResult.ItemsEarned)
            {
                ItemSO item = rewardItem.Key;
                int itemCount = rewardItem.Value;
                VisualElement itemSlotRoot = GenerateRewardItemSlot(item, itemCount);
                _itemRewardContainerToItem[itemSlotRoot] = item;
                _root.Add(itemSlotRoot);
            }

            _root.style.display = _root.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private VisualElement GenerateRewardItemSlot(ItemSO item, int itemCount)
        {
            // Spawn and setup item reward container
            VisualElement itemSlotRoot = _itemSlotTemplate.Instantiate();
            itemSlotRoot.AddToClassList(_itemSlotRootClassname);

            ItemSlotContainerUIController itemSlotContainerUIController = new(itemSlotRoot);
            itemSlotContainerUIController.SetItemSlot(
                new InventorySlot(item, itemCount, item.SlotTag.IsStackable() ? 99 : 1, item.SlotTag)
            );

            // Setup long hover event 
            LongHoverEventController<VisualElement> itemRewardLongHoverController = new(itemSlotRoot);
            itemRewardLongHoverController.onElementLongHovered += OnItemRewardContainerLongHovered;
            itemRewardLongHoverController.onElementLongUnhovered += OnItemRewardContainerLongUnhovered;

            return itemSlotRoot;
        }

        private void OnItemRewardContainerLongHovered(VisualElement itemRewardContainer)
        {
            if (_itemRewardContainerToItem.TryGetValue(itemRewardContainer, out ItemSO item))
            {
                _objectDetailsOverlayController.SetObject(item);
                _objectDetailsOverlayController.ShowOverlay(followMouse: true);
            }
        }

        private void OnItemRewardContainerLongUnhovered(VisualElement itemRewardContainer)
        {
            _objectDetailsOverlayController.HideOverlay();
        }
    }
}