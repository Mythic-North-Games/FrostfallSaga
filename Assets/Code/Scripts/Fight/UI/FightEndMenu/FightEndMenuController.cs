using System;
using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.InventorySystem.UI;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI.FightEndMenu
{
    /// <summary>
    ///     Responsible for controlling the fight end menu.
    /// </summary>
    public class FightEndMenuController
    {
        #region UXML Names and classes
        private static readonly string RESULT_LABEL_UI_NAME = "ResultLabel";
        private static readonly string ALLIES_STATE_CONTAINER_UI_NAME = "AlliesStateContainer";
        private static readonly string ENEMIES_STATE_CONTAINER_UI_NAME = "EnemiesStateContainer";
        private static readonly string STYCAS_REWARD_LAEBL_UI_NAME = "StycasRewardLabel";
        private static readonly string ITEMS_REWARD_CONTAINER_UI_NAME = "ItemsRewardContainer";
        private static readonly string CONTINUE_BUTTON_UI_NAME = "ContinueButton";
        private static readonly string OBJECT_DETAILS_PANEL_CONTAINER_UI_NAME = "ObjectDetailsPanelContainer";

        private static readonly string FIGHTER_STATE_CONTAINER_ROOT_CLASSNAME = "fighterStateRoot";
        private static readonly string ITEM_REWARD_CONTAINER_ROOT_CLASSNAME = "itemRewardContainerRoot";
        private static readonly string OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME = "objectDetailsPanelContainerHidden";
        private static readonly string OBJECT_DETAILS_EFFECT_LINE_CLASSNAME = "itemRewardDetailsEffectLine";
        #endregion

        public Action onContinueClicked;

        private readonly VisualTreeAsset _fighterStateContainerTemplate;
        private readonly VisualTreeAsset _itemRewardContainerTemplate;
        private readonly string _fightWonText;
        private readonly string _fightLostText;

        private readonly VisualElement _root;
        private readonly Label _resultLabel;
        private readonly VisualElement _alliesStateContainer;
        private readonly VisualElement _enemiesStateContainer;
        private readonly Label _stycasRewardLabel;
        private readonly VisualElement _itemsRewardContainer;
        private readonly VisualElement _objectDetailsPanelContainer;

        private readonly float _itemRewardLongHoverDuration;
        private readonly ObjectDetailsUIController _objectDetailsPanelController;
        private readonly Dictionary<VisualElement, ItemSO> _itemRewardContainerToItem = new();

        public FightEndMenuController(
            VisualElement root,
            VisualTreeAsset fighterStateContainerTemplate,
            VisualTreeAsset itemRewardContainerTemplate,
            VisualTreeAsset statContainerTemplate,
            Color itemRewardStatsColor,
            float itemRewardLongHoverDuration = 0.5f,
            string fightWonText = "Enemies defeated",
            string fightLostText = "You have been defeated"
        )
        {
            _root = root;
            _fighterStateContainerTemplate = fighterStateContainerTemplate;
            _itemRewardContainerTemplate = itemRewardContainerTemplate;
            _fightWonText = fightWonText;
            _fightLostText = fightLostText;

            // Get ui elements
            _resultLabel = root.Q<Label>(RESULT_LABEL_UI_NAME);
            _alliesStateContainer = root.Q<VisualElement>(ALLIES_STATE_CONTAINER_UI_NAME);
            _enemiesStateContainer = root.Q<VisualElement>(ENEMIES_STATE_CONTAINER_UI_NAME);
            _stycasRewardLabel = root.Q<Label>(STYCAS_REWARD_LAEBL_UI_NAME);
            _itemsRewardContainer = root.Q<VisualElement>(ITEMS_REWARD_CONTAINER_UI_NAME);
            _objectDetailsPanelContainer = root.Q<VisualElement>(OBJECT_DETAILS_PANEL_CONTAINER_UI_NAME);

            Button continueButton = root.Q<Button>(CONTINUE_BUTTON_UI_NAME);
            continueButton.RegisterCallback<ClickEvent>(_ => onContinueClicked?.Invoke());

            _objectDetailsPanelController = new ObjectDetailsUIController(
                root: _objectDetailsPanelContainer,
                statContainerTemplate: statContainerTemplate,
                effectLineClassname: OBJECT_DETAILS_EFFECT_LINE_CLASSNAME,
                statValueColor: itemRewardStatsColor
            );
            HideObjectDetailsPanel();
        }

        public void Setup(
            Fighter[] allies,
            Fighter[] enemies,
            bool alliesHaveWon,
            int earnedStycasCount,
            List<ItemSO> earnedItems
        )
        {
            SetupResultLabel(alliesHaveWon);
            SetupTeamFightersState(_alliesStateContainer, allies);
            SetupTeamFightersState(_enemiesStateContainer, enemies);
            SetupStycasRewardLabel(earnedStycasCount);
            SetupItemsRewardContainer(earnedItems);
        }

        public void SetVisible(bool visible)
        {
            _root.visible = visible;
        }

        private void SetupResultLabel(bool alliesHaveWon)
        {
            _resultLabel.text = alliesHaveWon ? _fightWonText : _fightLostText;
        }

        private void SetupTeamFightersState(VisualElement root, Fighter[] fighters)
        {
            // Clear previous fighters
            root.Clear();

            // Setup fighters
            foreach (Fighter fighter in fighters)
            {
                VisualElement fighterStateContainer = _fighterStateContainerTemplate.Instantiate();
                fighterStateContainer.AddToClassList(FIGHTER_STATE_CONTAINER_ROOT_CLASSNAME);
                FighterStateContainerUIController.Setup(fighter, fighterStateContainer);
                root.Add(fighterStateContainer);
            }
        }

        private void SetupStycasRewardLabel(int stycasEarnedCount)
        {
            _stycasRewardLabel.text = $"+ <b>{stycasEarnedCount}</b> stycas";
        }

        private void SetupItemsRewardContainer(List<ItemSO> earnedItems)
        {
            // Clear previous items
            _itemsRewardContainer.Clear();
            _itemRewardContainerToItem.Clear();

            // Get item and their count
            Dictionary<ItemSO, int> itemsCount = new();
            foreach (ItemSO item in earnedItems)
            {
                if (itemsCount.ContainsKey(item))
                    itemsCount[item]++;
                else
                    itemsCount[item] = 1;
            }

            // Setup items
            foreach (KeyValuePair<ItemSO, int> itemCount in itemsCount)
            {
                // Spawn and setup item reward container
                VisualElement itemRewardContainer = _itemRewardContainerTemplate.Instantiate();
                itemRewardContainer.AddToClassList(ITEM_REWARD_CONTAINER_ROOT_CLASSNAME);
                ItemRewardContainerUIController.Setup(
                    root: itemRewardContainer,
                    item: itemCount.Key,
                    count: itemCount.Value
                );

                // Setup long hover event 
                _itemRewardContainerToItem[itemRewardContainer] = itemCount.Key;
                LongHoverEventController<VisualElement> itemRewardLongHoverController = new(itemRewardContainer, _itemRewardLongHoverDuration);
                itemRewardLongHoverController.onElementLongHovered += OnItemRewardContainerLongHovered;
                itemRewardLongHoverController.onElementLongUnhovered += OnItemRewardContainerLongUnhovered;

                // Add item reward container to the container
                _itemsRewardContainer.Add(itemRewardContainer);
            }
        }

        private void OnItemRewardContainerLongHovered(VisualElement itemRewardContainer)
        {
            if (_itemRewardContainerToItem.TryGetValue(itemRewardContainer, out ItemSO item))
            {
                _objectDetailsPanelController.Setup(
                    icon: item.IconSprite,
                    name: item.Name,
                    description: item.Description,
                    stats: GetItemStats(item),
                    primaryEffectsTitle: GetItemEffectsTitle(item),
                    primaryEffects: GetItemEffects(item)
                );
                ShowObjectDetailsPanel();
            }
        }

        private void OnItemRewardContainerLongUnhovered(VisualElement itemRewardContainer)
        {
            HideObjectDetailsPanel();
        }

        private void ShowObjectDetailsPanel()
        {
            _objectDetailsPanelContainer.RemoveFromClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
        }

        private void HideObjectDetailsPanel()
        {
            _objectDetailsPanelContainer.AddToClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
        }

        private Dictionary<Sprite, string> GetItemStats(ItemSO item)
        {
            if (item is AEquipment equipment) return equipment.GetStatsUIData();
            else return null;
        }

        private List<string> GetItemEffects(ItemSO item)
        {
            if (item is AEquipment equipment) return equipment.GetSpecialEffectsUIData();
            else if (item is AConsumable consumable) return consumable.GetEffectsUIData();
            else return null;
        }

        private string GetItemEffectsTitle(ItemSO item)
        {
            if (item is AEquipment) return "Special effects";
            else if (item is AConsumable) return "Effects";
            else return null;
        }
    }
}