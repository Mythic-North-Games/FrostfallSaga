using System;
using System.Collections.Generic;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;

namespace FrostfallSaga.Fight.UI.FightEndMenu
{
    /// <summary>
    /// Responsible for controlling the fight end menu.
    /// </summary>
    public class FightEndMenuController
    {
        private static readonly float SECTION_DISPLAY_DELAY = 0.1f;

        #region UXML Names and classes
        private static readonly string RESULT_CONTAINER_UI_NAME = "ResultContainer";
        private static readonly string RESULT_LABEL_UI_NAME = "ResultLabel";
        private static readonly string ALLIES_STATE_CONTAINER_UI_NAME = "AlliesStateContainer";
        private static readonly string ENEMIES_STATE_CONTAINER_UI_NAME = "EnemiesStateContainer";
        private static readonly string REWARDS_SECTION_CONTAINER_UI_NAME = "RewardsContainer";
        private static readonly string REWARDS_CONTENT_CONTAINER_UI_NAME = "RewardsContentContainer";
        private static readonly string CONTINUE_BUTTON_CONTAINER_UI_NAME = "ContinueButtonContainer";
        private static readonly string CONTINUE_BUTTON_UI_NAME = "ContinueButton";

        private static readonly string CONTAINER_HIDDEN_CLASSNAME = "fightEndMenuContainerHidden";
        private static readonly string FIGHTER_STATE_CONTAINER_ROOT_CLASSNAME = "fighterStateRoot";
        private static readonly string ITEM_REWARD_CONTAINER_ROOT_CLASSNAME = "itemRewardContainerRoot";
        private static readonly string SECTION_HIDDEN_CLASSNAME = "sectionHidden";
        #endregion

        public Action onContinueClicked;

        private readonly VisualElement _root;
        private readonly VisualElement _resultContainer;
        private readonly Label _resultLabel;
        private readonly string _fightLostText;
        private readonly string _fightWonText;
        private readonly VisualElement _alliesStateContainer;
        private readonly VisualElement _enemiesStateContainer;
        private readonly VisualElement _rewardsSectionContainer;
        private readonly VisualElement _continueButtonContainer;
        private readonly Button _continueButton;
        private readonly VisualTreeAsset _fighterStateContainerTemplate;
        private readonly ItemRewardsListUIController _itemRewardsListUIController;

        public FightEndMenuController(
            VisualElement root,
            VisualTreeAsset fighterStateContainerTemplate,
            VisualTreeAsset itemRewardContainerTemplate,
            VisualTreeAsset statContainerTemplate,
            VisualTreeAsset rewardItemDetailsOverlayTemplate,
            string fightWonText = "Enemies defeated",
            string fightLostText = "You have been defeated"
        )
        {
            _root = root;
            _fightWonText = fightWonText;
            _fightLostText = fightLostText;
            _fighterStateContainerTemplate = fighterStateContainerTemplate;

            // Get ui elements
            _resultContainer = root.Q<VisualElement>(RESULT_CONTAINER_UI_NAME);
            _resultLabel = root.Q<Label>(RESULT_LABEL_UI_NAME);
            _alliesStateContainer = root.Q<VisualElement>(ALLIES_STATE_CONTAINER_UI_NAME);
            _enemiesStateContainer = root.Q<VisualElement>(ENEMIES_STATE_CONTAINER_UI_NAME);
            _rewardsSectionContainer = root.Q<VisualElement>(REWARDS_SECTION_CONTAINER_UI_NAME);
            _continueButtonContainer = root.Q<VisualElement>(CONTINUE_BUTTON_CONTAINER_UI_NAME);
            _continueButton = root.Q<Button>(CONTINUE_BUTTON_UI_NAME);
            _continueButton.RegisterCallback<ClickEvent>(_ => onContinueClicked?.Invoke());

            _itemRewardsListUIController = new ItemRewardsListUIController(
                root.Q<VisualElement>(REWARDS_CONTENT_CONTAINER_UI_NAME),
                itemRewardContainerTemplate,
                ITEM_REWARD_CONTAINER_ROOT_CLASSNAME,
                rewardItemDetailsOverlayTemplate,
                statContainerTemplate
            );
        }

        public void Setup(
            Fighter[] allies,
            Fighter[] enemies,
            bool alliesHaveWon,
            int earnedStycasCount,
            Dictionary<ItemSO, int> earnedItems
        )
        {
            SetupResultLabel(alliesHaveWon);
            SetupTeamFightersState(_alliesStateContainer, allies);
            SetupTeamFightersState(_enemiesStateContainer, enemies);
            _rewardsSectionContainer.style.display = alliesHaveWon ? DisplayStyle.Flex : DisplayStyle.None;
            if (alliesHaveWon)
            {
                SetupItemsRewardContainer(earnedItems, earnedStycasCount);
            }
        }

        public IEnumerator DisplayMenu()
        {
            _root.RemoveFromClassList(CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(SECTION_DISPLAY_DELAY);

            _resultContainer.RemoveFromClassList(SECTION_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(SECTION_DISPLAY_DELAY);

            _alliesStateContainer.RemoveFromClassList(SECTION_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(SECTION_DISPLAY_DELAY);

            _enemiesStateContainer.RemoveFromClassList(SECTION_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(SECTION_DISPLAY_DELAY);

            _rewardsSectionContainer.RemoveFromClassList(SECTION_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(SECTION_DISPLAY_DELAY);

            _continueButtonContainer.RemoveFromClassList(SECTION_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(SECTION_DISPLAY_DELAY);

            UIUtils.SetHierachyPickingMode(_root, PickingMode.Position);
        }

        public void HideMenu()
        {
            UIUtils.SetHierachyPickingMode(_root, PickingMode.Ignore);
            _root.AddToClassList(CONTAINER_HIDDEN_CLASSNAME);
            _resultContainer.AddToClassList(SECTION_HIDDEN_CLASSNAME);
            _alliesStateContainer.AddToClassList(SECTION_HIDDEN_CLASSNAME);
            _enemiesStateContainer.AddToClassList(SECTION_HIDDEN_CLASSNAME);
            _rewardsSectionContainer.AddToClassList(SECTION_HIDDEN_CLASSNAME);
            _continueButtonContainer.AddToClassList(SECTION_HIDDEN_CLASSNAME);
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
                CharacterStateContainerUIController.Setup(
                    fighterStateContainer,
                    fighter.DiamondIcon,
                    fighter.GetHealth(),
                    fighter.GetMaxHealth()
                );
                root.Add(fighterStateContainer);
            }
        }

        private void SetupItemsRewardContainer(Dictionary<ItemSO, int> earnedItems, int earnedStycasCount)
        {
            _itemRewardsListUIController.UpdateItemRewardsList(
                new(
                    stycasEarned: earnedStycasCount,
                    itemsEarned: earnedItems
                )
            );
        }
    }
}