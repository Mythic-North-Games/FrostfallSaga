using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.InventorySystem.UI
{
    public class BookInventoryMenuUIController : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset equippedPanelTemplate;
        [SerializeField] private VisualTreeAsset bagPanelTemplate;
        [SerializeField] private VisualTreeAsset statContainerTemplate;
        [SerializeField] private Color statValueColor = new(0.2f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color statIconColor;
        [SerializeField] private EntityConfigurationSO devHero;
        private InventoryBagPanelUIController _bagPanelUIController;

        private EntityConfigurationSO _currentHeroEntityConf;
        private Inventory _currentHeroInventory;
        private InventoryEquippedPanelUIController _equippedPanelUIController;
        private HeroChooserUIController _heroChooserUIController;
        private ItemSlotContainerUIController _selectedItemSlotContainerUIController;

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            if (!equippedPanelTemplate)
            {
                Debug.LogError("Equipped Panel Template is not set in the inspector.");
                return;
            }

            if (!bagPanelTemplate)
            {
                Debug.LogError("Bag Panel Template is not set in the inspector.");
                return;
            }

            if (!statContainerTemplate)
            {
                Debug.LogError("Item Details Stat Container Template is not set in the inspector.");
            }
        }

        #endregion

        public override void SetupMenu()
        {
            // Get the first hero in the team to display its inventory
            List<Hero> heroes = HeroTeam.Instance.Heroes;
            _currentHeroEntityConf = HeroTeam.Instance.Heroes[0].EntityConfiguration;
            if (devHero)
            {
                _currentHeroEntityConf = devHero;
            }

            _currentHeroInventory =
                ((PersistedFighterConfigurationSO)_currentHeroEntityConf.FighterConfiguration).Inventory;

            // Setup equipped panel (left page)
            VisualElement equippedPanelRoot = equippedPanelTemplate.Instantiate();
            equippedPanelRoot.StretchToParentSize();
            _equippedPanelUIController = new InventoryEquippedPanelUIController(equippedPanelRoot);
            _equippedPanelUIController.onItemSlotSelected += OnItemSlotSelected;
            _equippedPanelUIController.onItemSlotUnequipClicked += OnItemSlotUnequipClicked;
            _equippedPanelUIController.UpdateAllPanelHero(_currentHeroEntityConf);

            // Setup hero chooser (left page)
            _heroChooserUIController = new HeroChooserUIController(equippedPanelRoot);
            _heroChooserUIController.SetHeroes(heroes);
            _heroChooserUIController.ActivateHero(heroes[0]);
            _heroChooserUIController.onHeroChosen += OnHeroChosen;

            // Setup bag panel (right page)
            VisualElement bagPanelRoot = bagPanelTemplate.Instantiate();
            bagPanelRoot.StretchToParentSize();
            _bagPanelUIController =
                new InventoryBagPanelUIController(bagPanelRoot, statContainerTemplate, statValueColor, statIconColor);
            _bagPanelUIController.onItemSlotSelected += OnItemSlotSelected;
            _bagPanelUIController.onItemSlotEquipClicked += OnItemSlotEquipClicked;
            _bagPanelUIController.SetInventory(_currentHeroInventory);

            _leftPageContainer.Add(equippedPanelRoot);
            _rightPageContainer.Add(bagPanelRoot);
        }

        public override void ClearMenu()
        {
            base.ClearMenu();
            _equippedPanelUIController = null;
            _bagPanelUIController = null;
        }

        private void OnItemSlotSelected(ItemSlotContainerUIController selectedItemSlotController)
        {
            if (_selectedItemSlotContainerUIController == selectedItemSlotController)
            {
                return;
            }
            _selectedItemSlotContainerUIController?.SetActive(false);

            _selectedItemSlotContainerUIController = selectedItemSlotController;
            ItemSO selectedItem = selectedItemSlotController.CurrentItemSlot.Item;
            if (!selectedItem)
            {
                _bagPanelUIController.HideItemDetails();
            }
            else
            {
                selectedItemSlotController.SetActive(true);
                _bagPanelUIController.DisplayItemDetails(selectedItem);
            }
        }

        private void OnItemSlotEquipClicked(ItemSlotContainerUIController selectedItemSlotController)
        {
            ItemSO selectedItem = selectedItemSlotController.CurrentItemSlot.Item;

            // If item is not equippable, do nothing
            if (!selectedItem || !selectedItem.IsEquippable())
            {
                selectedItemSlotController.PlayCannotEquipAnimation(_uiDoc);
                return;
            }

            _bagPanelUIController.DisplayItemDetails(selectedItem); // Needs to be before equip because equipping will clear the item slot

            // Equip the item and update UI depending on its type
            if (selectedItem is AArmor or AWeapon)
            {
                _currentHeroInventory.EquipEquipment(selectedItem);
                _equippedPanelUIController.UpdateEquipment(_currentHeroInventory);
            }
            else if (selectedItem is AConsumable)
            {
                _currentHeroInventory.AddConsumableSlotToQuickAccess(selectedItemSlotController.CurrentItemSlot);
                _equippedPanelUIController.UpdateQuickAccessSlots(_currentHeroInventory);
            }

            _bagPanelUIController.SetInventory(_currentHeroInventory);
        }

        private void OnItemSlotUnequipClicked(ItemSlotContainerUIController selectedItemSlotController)
        {
            ItemSO selectedItem = selectedItemSlotController.CurrentItemSlot.Item;

            // If item is not equippable, do nothing
            if (!selectedItem || !selectedItem.IsEquippable())
            {
                selectedItemSlotController.PlayCannotEquipAnimation(_uiDoc);
                return;
            }

            _bagPanelUIController.DisplayItemDetails(selectedItem); // Needs to be before equip because unequipping will clear the item slot
            _currentHeroInventory.UnequipItem(selectedItemSlotController.CurrentItemSlot);

            if (selectedItem is AArmor or AWeapon)
            {
                _equippedPanelUIController.UpdateEquipment(_currentHeroInventory);
            }
            else if (selectedItem is AConsumable)
            {
                _equippedPanelUIController.UpdateQuickAccessSlots(_currentHeroInventory);
            }
            _bagPanelUIController.SetInventory(_currentHeroInventory);
        }

        private void OnHeroChosen(Hero chosenHero)
        {
            // If choosing the same hero, do nothing
            if (chosenHero.EntityConfiguration == _currentHeroEntityConf)
            {
                return;
            }

            _bagPanelUIController.HideItems();
            _bagPanelUIController.HideItemDetails();
            _selectedItemSlotContainerUIController?.SetActive(false);
            _equippedPanelUIController.HideItems();

            _selectedItemSlotContainerUIController = null;
            _currentHeroEntityConf = chosenHero.EntityConfiguration;
            _currentHeroInventory =
                ((PersistedFighterConfigurationSO)_currentHeroEntityConf.FighterConfiguration).Inventory;
            StartCoroutine(UpdatePanels());
        }

        private IEnumerator UpdatePanels()
        {
            _equippedPanelUIController.UpdateAllPanelHero(_currentHeroEntityConf);
            yield return new WaitForSeconds(0.25f);
            _bagPanelUIController.SetInventory(_currentHeroInventory);
        }
    }
}