using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class ConsumablesUIController
    {
        private readonly List<InventorySlot> _consumablesFromBagInQuickAccess = new();
        private readonly List<ItemSlotContainerUIController> _consumableSlotControllers = new();
        private readonly VisualTreeAsset _consumableSlotTemplate;
        private readonly Button _expandConsumablesButton;
        private readonly VisualElement _extraConsumablesSlotsContainer;

        private readonly VisualElement _root;
        public Action<InventorySlot> onConsumableLongHovered;
        public Action<InventorySlot> onConsumableLongUnhovered;

        public Action<InventorySlot> onConsumableUsed;

        public ConsumablesUIController(VisualElement root, VisualTreeAsset consumableSlotTemplate)
        {
            _root = root;
            _consumableSlotTemplate = consumableSlotTemplate;

            // Setup extra consumables slots container
            _extraConsumablesSlotsContainer = _root.Q<VisualElement>(EXTRA_CONSUMABLES_SLOTS_CONTAINER_UI_NAME);
            _extraConsumablesSlotsContainer.AddToClassList(EXTRA_CONSUMABLES_SLOTS_CONTAINER_HIDDEN_CLASSNAME);
            _extraConsumablesSlotsContainer.Clear();

            // Setup expand consumables button
            _expandConsumablesButton = _root.Q<Button>(EXPAND_CONSUMABLES_BUTTON_UI_NAME);
            _expandConsumablesButton.RegisterCallback<ClickEvent>(OnExpandConsumablesButtonClicked);

            // Setup quick access consumable slots
            foreach (VisualElement consumableSlot in _root.Q<VisualElement>(CONSUMABLES_SLOTS_CONTAINER_UI_NAME)
                         .Children())
            {
                _consumableSlotControllers.Add(new(consumableSlot.Children().First()));
            }
        }

        public void UpdateConsumables(Inventory playingFighterInventory)
        {
            SetupQuickAccessSlots(playingFighterInventory);
            SetupExtraConsumableSlots(playingFighterInventory);
        }

        private void SetupQuickAccessSlots(Inventory playingFighterInventory)
        {
            InventorySlot[] consumablesQuickAccessSlots = playingFighterInventory.ConsumablesQuickAccessSlots
                .Where(slot => slot.Item != null)
                .ToArray();
            InventorySlot[] consumablesBagSlots = playingFighterInventory.GetConsumableSlotsInBag();
            _consumablesFromBagInQuickAccess.Clear();
            for (int i = 0; i < _consumableSlotControllers.Count; i++)
            {
                ItemSlotContainerUIController consumableSlotController = _consumableSlotControllers[i];
                InventorySlot consumableSlotToSet = null;

                // Get the consumable slot to set
                if (i < consumablesQuickAccessSlots.Length)
                {
                    consumableSlotToSet = consumablesQuickAccessSlots[i];
                }
                else if (i - consumablesQuickAccessSlots.Length < consumablesBagSlots.Length)
                {
                    consumableSlotToSet = consumablesBagSlots[i - consumablesQuickAccessSlots.Length];
                    _consumablesFromBagInQuickAccess.Add(consumableSlotToSet);
                }

                if (consumableSlotToSet == null)
                {
                    consumableSlotController.SetItemSlot(null);
                    consumableSlotController.SetEnabled(false);
                    continue;
                }

                // Setup consumable slot
                consumableSlotController.SetItemSlot(consumableSlotToSet);
                consumableSlotController.onItemSelected += OnConsumableSlotClicked;
                consumableSlotController.SetEnabled(true);

                // Setup consumable slot long hover events
                LongHoverEventController<VisualElement> longHoverEventController = new(consumableSlotController.Root);
                longHoverEventController.onElementLongHovered += (_) =>
                {
                    onConsumableLongHovered?.Invoke(consumableSlotToSet);
                };
                longHoverEventController.onElementLongUnhovered += (_) =>
                {
                    onConsumableLongUnhovered?.Invoke(consumableSlotToSet);
                };
            }
        }

        private void SetupExtraConsumableSlots(Inventory playingFighterInventory)
        {
            // Get the extra consumables slots that are not in the quick access
            List<InventorySlot> extraConsumablesSlot = playingFighterInventory
                .GetConsumableSlotsInBag()
                .Where(consumableSlot => !_consumablesFromBagInQuickAccess.Contains(consumableSlot))
                .ToList();

            // If there are no extra consumables, disable expand button
            if (extraConsumablesSlot.Count == 0)
            {
                _expandConsumablesButton.SetEnabled(false);
                _expandConsumablesButton.pickingMode = PickingMode.Ignore;
                return;
            }

            // Otherwise, enable the expand button
            _expandConsumablesButton.SetEnabled(true);
            _expandConsumablesButton.pickingMode = PickingMode.Position;

            // Setup extra consumables slots
            _extraConsumablesSlotsContainer.Clear();
            foreach (InventorySlot consumableSlot in extraConsumablesSlot)
            {
                // Create a new consumable slot
                VisualElement extraConsumableSlotRoot = _consumableSlotTemplate.CloneTree();
                extraConsumableSlotRoot.AddToClassList(EXTRA_CONSUMABLES_SLOT_ROOT_CLASSNAME);

                ItemSlotContainerUIController newConsumableSlotController = new(extraConsumableSlotRoot);
                newConsumableSlotController.SetItemSlot(consumableSlot);
                newConsumableSlotController.onItemSelected += OnConsumableSlotClicked;

                // Setup extra consumable slot long hover events
                LongHoverEventController<VisualElement> longHoverEventController = new(extraConsumableSlotRoot);
                longHoverEventController.onElementLongHovered += (_) =>
                {
                    onConsumableLongHovered?.Invoke(consumableSlot);
                };
                longHoverEventController.onElementLongUnhovered += (_) =>
                {
                    onConsumableLongUnhovered?.Invoke(consumableSlot);
                };

                _extraConsumablesSlotsContainer.Add(extraConsumableSlotRoot);
            }
        }

        private void OnConsumableSlotClicked(ItemSlotContainerUIController clickedConsumableSlot)
        {
            if (clickedConsumableSlot.CurrentItemSlot.Item is ConsumableSO) // Should always be true
            {
                onConsumableUsed?.Invoke(clickedConsumableSlot.CurrentItemSlot);
            }
        }

        private void OnExpandConsumablesButtonClicked(ClickEvent clickEvent)
        {
            _extraConsumablesSlotsContainer.ToggleInClassList(EXTRA_CONSUMABLES_SLOTS_CONTAINER_HIDDEN_CLASSNAME);
            if (_extraConsumablesSlotsContainer.ClassListContains(EXTRA_CONSUMABLES_SLOTS_CONTAINER_HIDDEN_CLASSNAME))
            {
                _expandConsumablesButton.text = "+";
                _extraConsumablesSlotsContainer.pickingMode = PickingMode.Ignore;
            }
            else
            {
                _expandConsumablesButton.text = "-";
                _extraConsumablesSlotsContainer.pickingMode = PickingMode.Position;
            }
        }

        #region UXML Element names and classes

        private static readonly string CONSUMABLES_SLOTS_CONTAINER_UI_NAME = "ConsumableSlotsContainer";
        private static readonly string EXPAND_CONSUMABLES_BUTTON_UI_NAME = "ExpandConsumablesButton";
        private static readonly string EXTRA_CONSUMABLES_SLOTS_CONTAINER_UI_NAME = "ExtraConsumablesSlotsContainer";

        private static readonly string EXTRA_CONSUMABLES_SLOTS_CONTAINER_HIDDEN_CLASSNAME =
            "extraConsumablesSlotsContainerHidden";

        private static readonly string EXTRA_CONSUMABLES_SLOT_ROOT_CLASSNAME = "extraConsumableSlotRoot";

        #endregion
    }
}