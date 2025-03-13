using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.UI;
using System.Collections.Generic;

namespace FrostfallSaga.InventorySystem.UI
{
    public class BookInventoryMenuUIController : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset _equippedPanelTemplate;
        [SerializeField] private VisualTreeAsset _bagPanelTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private EntityConfigurationSO _devHero;

        private EntityConfigurationSO _currentHeroEntityConf;
        private Inventory _currentHeroInventory;
        private InventoryEquippedPanelUIController _equippedPanelUIController;
        private InventoryBagPanelUIController _bagPanelUIController;
        private HeroChooserUIController _heroChooserUIController;

        public override void SetupMenu()
        {
            // Get the first hero in the team to display its inventory
            List<Hero> heroes = HeroTeam.Instance.Heroes;
            _currentHeroEntityConf = HeroTeam.Instance.Heroes[0].EntityConfiguration;
            if (_devHero != null)
            {
                _currentHeroEntityConf = _devHero;
            }
            _currentHeroInventory = ((PersistedFighterConfigurationSO)_currentHeroEntityConf.FighterConfiguration).Inventory;

            // Setup equipped panel (left page)
            VisualElement equippedPanelRoot = _equippedPanelTemplate.Instantiate();
            equippedPanelRoot.StretchToParentSize();
            _equippedPanelUIController = new InventoryEquippedPanelUIController(equippedPanelRoot);
            _equippedPanelUIController.onItemSlotSelected += OnItemSlotSelected;
            _equippedPanelUIController.onItemSlotUnequipClicked += OnItemSlotUnequipClicked;
            _equippedPanelUIController.SetHero(_currentHeroEntityConf);

            // Setup hero chooser (left page)
            _heroChooserUIController = new HeroChooserUIController(equippedPanelRoot);
            _heroChooserUIController.SetHeroes(heroes);
            _heroChooserUIController.onHeroChosen += OnHeroChosen;

            // Setup bag panel (right page)
            VisualElement bagPanelRoot = _bagPanelTemplate.Instantiate();
            bagPanelRoot.StretchToParentSize();
            _bagPanelUIController = new InventoryBagPanelUIController(bagPanelRoot, _statContainerTemplate);
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

        private void OnItemSlotSelected(InventorySlot selectedItemSlot)
        {
            if (selectedItemSlot.Item == null)
            {
                _bagPanelUIController.ClearItemDetails();
            }
            else
            {
                _bagPanelUIController.DisplayItemDetails(selectedItemSlot.Item);
            }
        }

        private void OnItemSlotEquipClicked(InventorySlot selectedItemSlot)
        {
            // If item is not equippable, do nothing
            if (selectedItemSlot.Item == null || selectedItemSlot.Item.SlotTag == EItemSlotTag.BAG)
            {
                return;
            }

            _bagPanelUIController.DisplayItemDetails(selectedItemSlot.Item); // Needs to be before equip because equipping will clear the item slot
            _currentHeroInventory.EquipItem(selectedItemSlot.Item);
            _equippedPanelUIController.SetHero(_currentHeroEntityConf);
            _bagPanelUIController.SetInventory(_currentHeroInventory);
        }

        private void OnItemSlotUnequipClicked(InventorySlot selectedItemSlot)
        {
            // If item is not equippable, do nothing
            if (selectedItemSlot.Item == null || selectedItemSlot.Item.SlotTag == EItemSlotTag.BAG)
            {
                return;
            }

            _bagPanelUIController.DisplayItemDetails(selectedItemSlot.Item); // Needs to be before equip because unequipping will clear the item slot
            _currentHeroInventory.UnequipItem(selectedItemSlot.Item);
            _equippedPanelUIController.SetHero(_currentHeroEntityConf);
            _bagPanelUIController.SetInventory(_currentHeroInventory);
        }

        private void OnHeroChosen(Hero chosenHero)
        {
            // If choosing the same hero, do nothing
            if (chosenHero.EntityConfiguration == _currentHeroEntityConf)
            {
                return;
            }

            _currentHeroEntityConf = chosenHero.EntityConfiguration;
            _currentHeroInventory = ((PersistedFighterConfigurationSO)_currentHeroEntityConf.FighterConfiguration).Inventory;
            _equippedPanelUIController.SetHero(_currentHeroEntityConf);
            _bagPanelUIController.SetInventory(_currentHeroInventory);
            _bagPanelUIController.ClearItemDetails();
        }

        #region Setup
        protected override void Awake()
        {
            base.Awake();

            if (_equippedPanelTemplate == null)
            {
                Debug.LogError("Equipped Panel Template is not set in the inspector.");
            }

            if (_bagPanelTemplate == null)
            {
                Debug.LogError("Bag Panel Template is not set in the inspector.");
            }

            if (_statContainerTemplate == null)
            {
                Debug.LogError("Item Details Stat Container Template is not set in the inspector.");
            }
        }
        #endregion
    }
}