using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryEquippedPanelUIController
    {
        private readonly ItemSlotContainerUIController _bootsSlotContainer;
        private readonly ItemSlotContainerUIController _chestSlotContainer;
        private readonly ItemSlotContainerUIController _gauntletSlotContainer;
        private readonly ItemSlotContainerUIController _helmetSlotContainer;
        private readonly InventoryHeroRenderTextureSceneController _heroRenderTextureSceneController;
        private readonly List<ItemSlotContainerUIController> _quickAccessSlots = new();

        private readonly VisualElement _root;
        private readonly ItemSlotContainerUIController _weaponSlotContainer;

        private EntityConfigurationSO _currentHeroEntityConf;
        private PersistedFighterConfigurationSO _currentHeroFighterConf;

        public Action<InventorySlot> onItemSlotSelected;
        public Action<InventorySlot> onItemSlotUnequipClicked;

        public InventoryEquippedPanelUIController(
            VisualElement root,
            InventoryHeroRenderTextureSceneController heroRenderTextureSceneController
        )
        {
            _root = root;
            _heroRenderTextureSceneController = heroRenderTextureSceneController;

            // Retrieve UI elements and create controllers for each equipment slot
            _weaponSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_WEAPON_SLOT_CONTAINER_UI_NAME));
            _helmetSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_HELMET_SLOT_CONTAINER_UI_NAME));
            _chestSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_CHEST_SLOT_CONTAINER_UI_NAME));
            _gauntletSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_GAUNTLET_SLOT_CONTAINER_UI_NAME));
            _bootsSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_BOOTS_SLOT_CONTAINER_UI_NAME));

            // Subscribe to events for each equipment slot
            _weaponSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _helmetSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _chestSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _gauntletSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _bootsSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);

            _weaponSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _helmetSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _chestSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _gauntletSlotContainer.onItemEquipToggled +=
                (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _bootsSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);

            // Retrieve and subscribe to events for each quick access slot
            foreach (VisualElement quickAccessSlot in _root.Q<VisualElement>(QUICK_ACCESS_SLOTS_CONTAINER_UI_NAME)
                         .Children())
            {
                ItemSlotContainerUIController quickAccessSlotController = new(quickAccessSlot);
                quickAccessSlotController.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
                quickAccessSlotController.onItemEquipToggled +=
                    (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
                _quickAccessSlots.Add(quickAccessSlotController);
            }
        }

        public void SetHero(EntityConfigurationSO heroEntityConf)
        {
            _currentHeroEntityConf = heroEntityConf;
            _currentHeroFighterConf = (PersistedFighterConfigurationSO)_currentHeroEntityConf.FighterConfiguration;
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            _heroRenderTextureSceneController.SetupHeroModel(_currentHeroEntityConf.InventoryVisualPrefab);
            UpdateFighterName();
            UpdateEquipment(_currentHeroFighterConf.Inventory);
            UpdateQuickAccessSlots(_currentHeroFighterConf.Inventory);
        }

        private void UpdateFighterName()
        {
            _root.Q<Label>(FIGHTER_NAME_LABEL_UI_NAME).text = _currentHeroEntityConf.Name;
        }

        private void UpdateEquipment(Inventory inventory)
        {
            _weaponSlotContainer.SetItemSlot(inventory.WeaponSlot);
            _helmetSlotContainer.SetItemSlot(inventory.HelmetSlot);
            _chestSlotContainer.SetItemSlot(inventory.ChestplateSlot);
            _gauntletSlotContainer.SetItemSlot(inventory.GauntletsSlot);
            _bootsSlotContainer.SetItemSlot(inventory.BootsSlot);
        }

        private void UpdateQuickAccessSlots(Inventory inventory)
        {
            for (int i = 0; i < _quickAccessSlots.Count; i++)
            {
                _quickAccessSlots[i].SetItemSlot(inventory.ConsumablesQuickAccessSlots[i]);
            }
        }

        #region UI Elements Names & Classes

        private static readonly string FIGHTER_NAME_LABEL_UI_NAME = "FighterNameLabel";
        private static readonly string EQUIPPED_WEAPON_SLOT_CONTAINER_UI_NAME = "EquippedWeaponSlotContainer";
        private static readonly string EQUIPPED_HELMET_SLOT_CONTAINER_UI_NAME = "EquippedHelmetRuneSlotContainer";
        private static readonly string EQUIPPED_CHEST_SLOT_CONTAINER_UI_NAME = "EquippedChestplateRuneSlotContainer";
        private static readonly string EQUIPPED_GAUNTLET_SLOT_CONTAINER_UI_NAME = "EquippedGauntletRuneSlotContainer";
        private static readonly string EQUIPPED_BOOTS_SLOT_CONTAINER_UI_NAME = "EquippedBootsRuneSlotContainer";
        private static readonly string QUICK_ACCESS_SLOTS_CONTAINER_UI_NAME = "QuickAccessSlotsContainer";

        #endregion
    }
}