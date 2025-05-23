using System;
using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryEquippedPanelUIController
    {
        private const float DISPLAY_TRANSITION_DURATION = 0.25f;

        #region UI Elements Names & Classes
        private static readonly string FIGHTER_BACKGROUND_ILLUSTRATION_UI_NAME = "FighterBackgroundIllustration";
        private static readonly string FIGHTER_TPOSE_UI_NAME = "FighterTPose";
        private static readonly string FIGHTER_NAME_LABEL_UI_NAME = "FighterNameLabel";
        private static readonly string EQUIPPED_WEAPON_SLOT_CONTAINER_UI_NAME = "EquippedWeaponSlotContainer";
        private static readonly string EQUIPPED_HELMET_SLOT_CONTAINER_UI_NAME = "EquippedHelmetRuneSlotContainer";
        private static readonly string EQUIPPED_CHEST_SLOT_CONTAINER_UI_NAME = "EquippedChestplateRuneSlotContainer";
        private static readonly string EQUIPPED_LEGGINGS_SLOT_CONTAINER_UI_NAME = "EquippedLeggingsRuneSlotContainer";
        private static readonly string EQUIPPED_GAUNTLET_SLOT_CONTAINER_UI_NAME = "EquippedGauntletRuneSlotContainer";
        private static readonly string EQUIPPED_BOOTS_SLOT_CONTAINER_UI_NAME = "EquippedBootsRuneSlotContainer";
        private static readonly string QUICK_ACCESS_SLOTS_CONTAINER_UI_NAME = "QuickAccessSlotsContainer";

        private static readonly string FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME = "fighterNameLabelHidden";
        private static readonly string FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME = "fighterBackgroundIllustrationHidden";
        private static readonly string FIGHTER_EQUIPMENTS_CONTAINER_HIDDEN_CLASSNAME = "fighterTPoseHidden";
        #endregion

        private readonly VisualElement _fighterBackgroundIllustration;
        private readonly VisualElement _fighterEquipmentsContainer;
        private readonly ItemSlotContainerUIController _helmetSlotContainer;
        private readonly ItemSlotContainerUIController _chestSlotContainer;
        private readonly ItemSlotContainerUIController _gauntletSlotContainer;
        private readonly ItemSlotContainerUIController _leggingsSlotContainer;
        private readonly ItemSlotContainerUIController _bootsSlotContainer;
        private readonly ItemSlotContainerUIController _weaponSlotContainer;
        private readonly List<ItemSlotContainerUIController> _quickAccessSlots = new();

        public Action<ItemSlotContainerUIController> onItemSlotSelected;
        public Action<ItemSlotContainerUIController> onItemSlotUnequipClicked;

        private readonly VisualElement _root;
        private readonly Label _fighterNameLabel;

        private EntityConfigurationSO _currentHeroEntityConf;
        private PersistedFighterConfigurationSO _currentHeroFighterConf;

        private readonly CoroutineRunner _coroutineRunner;

        public InventoryEquippedPanelUIController(VisualElement root)
        {
            _root = root;
            _coroutineRunner = CoroutineRunner.Instance;

            // Retrieve UI elements and create controllers
            _fighterBackgroundIllustration = _root.Q<VisualElement>(FIGHTER_BACKGROUND_ILLUSTRATION_UI_NAME);
            _fighterEquipmentsContainer = _root.Q<VisualElement>(FIGHTER_TPOSE_UI_NAME);
            _fighterNameLabel = _root.Q<Label>(FIGHTER_NAME_LABEL_UI_NAME);
            
            _helmetSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_HELMET_SLOT_CONTAINER_UI_NAME));
            _chestSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_CHEST_SLOT_CONTAINER_UI_NAME));
            _gauntletSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_GAUNTLET_SLOT_CONTAINER_UI_NAME));
            _leggingsSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_LEGGINGS_SLOT_CONTAINER_UI_NAME));
            _bootsSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_BOOTS_SLOT_CONTAINER_UI_NAME));
            _weaponSlotContainer =
                new ItemSlotContainerUIController(_root.Q<VisualElement>(EQUIPPED_WEAPON_SLOT_CONTAINER_UI_NAME));

            // Subscribe to events for each equipment slot
            _helmetSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _chestSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _gauntletSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _leggingsSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _bootsSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
            _weaponSlotContainer.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);

            _helmetSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _chestSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _gauntletSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _leggingsSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _bootsSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
            _weaponSlotContainer.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);

            // Retrieve and subscribe to events for each quick access slot
            foreach (VisualElement quickAccessSlot in _root.Q<VisualElement>(QUICK_ACCESS_SLOTS_CONTAINER_UI_NAME).Children())
            {
                ItemSlotContainerUIController quickAccessSlotController = new(quickAccessSlot);
                quickAccessSlotController.onItemSelected += (selectedItem) => onItemSlotSelected?.Invoke(selectedItem);
                quickAccessSlotController.onItemEquipToggled += (selectedItem) => onItemSlotUnequipClicked?.Invoke(selectedItem);
                _quickAccessSlots.Add(quickAccessSlotController);
            }
        }

        public void UpdateAllPanelHero(EntityConfigurationSO heroEntityConf)
        {
            _currentHeroEntityConf = heroEntityConf;
            _currentHeroFighterConf = (PersistedFighterConfigurationSO)_currentHeroEntityConf.FighterConfiguration;
            _coroutineRunner.StartCoroutine(UpdatePanel());
        }

        public void UpdateEquipment(Inventory inventory)
        {
            _weaponSlotContainer.SetItemSlot(inventory.WeaponSlot);
            _helmetSlotContainer.SetItemSlot(inventory.HelmetSlot);
            _chestSlotContainer.SetItemSlot(inventory.ChestplateSlot);
            _gauntletSlotContainer.SetItemSlot(inventory.GauntletsSlot);
            _leggingsSlotContainer.SetItemSlot(inventory.LeggingsSlot);
            _bootsSlotContainer.SetItemSlot(inventory.BootsSlot);
        }

        public void UpdateQuickAccessSlots(Inventory inventory)
        {
            for (int i = 0; i < _quickAccessSlots.Count; i++)
            {
                _quickAccessSlots[i].SetItemSlot(inventory.ConsumablesQuickAccessSlots[i]);
            }
        }

        public void HideItems()
        {
            _helmetSlotContainer.HideItem();
            _chestSlotContainer.HideItem();
            _gauntletSlotContainer.HideItem();
            _leggingsSlotContainer.HideItem();
            _bootsSlotContainer.HideItem();
            _weaponSlotContainer.HideItem();
            _quickAccessSlots.ForEach(quickAccessSlot => quickAccessSlot.HideItem());
        }

        private IEnumerator UpdatePanel()
        {
            _coroutineRunner.StartCoroutine(UpdateIllustrations());
            _coroutineRunner.StartCoroutine(UpdateFighterName());
            yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            UpdateEquipment(_currentHeroFighterConf.Inventory);
            UpdateQuickAccessSlots(_currentHeroFighterConf.Inventory);
        }

        private IEnumerator UpdateIllustrations()
        {
            if (!_fighterBackgroundIllustration.ClassListContains(FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME))
            {
                _fighterBackgroundIllustration.AddToClassList(FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME);
                _fighterEquipmentsContainer.AddToClassList(FIGHTER_EQUIPMENTS_CONTAINER_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            }

            _fighterBackgroundIllustration.style.backgroundImage = new(_currentHeroEntityConf.InventoryBackgroundIllustration);
            _fighterEquipmentsContainer.style.backgroundImage = new(_currentHeroEntityConf.InventoryTPoseIllustration);

            yield return new WaitForSeconds(0.1f);

            _fighterBackgroundIllustration.RemoveFromClassList(FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME);
            _fighterEquipmentsContainer.RemoveFromClassList(FIGHTER_EQUIPMENTS_CONTAINER_HIDDEN_CLASSNAME);
        }

        private IEnumerator UpdateFighterName()
        {
            if (!_fighterNameLabel.ClassListContains(FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME))
            {
                _fighterNameLabel.AddToClassList(FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            }

            _root.Q<Label>(FIGHTER_NAME_LABEL_UI_NAME).text = _currentHeroEntityConf.Name;
            _fighterNameLabel.RemoveFromClassList(FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME);
        }
    }
}