using FrostfallSaga.Core.Quests;
using FrostfallSaga.Core.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class QuestRewardsUIController
    {
        #region UI Elements Names & Classes
        private static readonly string REWARDS_TITLE_LABEL_UI_NAME = "RewardsTitleLabel";
        private static readonly string REWARDS_CONTENT_CONTAINER_UI_NAME = "RewardsList";
        private static readonly string REWARDS_QUESTION_MARK_LABEL_UI_NAME = "QuestionMarkLabel";

        private static readonly string REWARD_ITEM_SLOT_CONTAINER_CLASSNAME = "rewardItemSlotContainer";
        #endregion

        private readonly VisualElement _root;
        private readonly Label _rewardsTitleLabel;
        private readonly VisualElement _rewardsContentContainer;
        private readonly VisualElement _rewardsQuestionMarkLabel;
        private readonly ItemRewardsListUIController _itemRewardsListUIController;

        public QuestRewardsUIController(
            VisualElement root,
            VisualTreeAsset itemSlotTemplate,
            VisualTreeAsset rewardItemDetailsOverlayTemplate,
            VisualTreeAsset statContainerTemplate
        )
        {
            _root = root;
            _rewardsTitleLabel = _root.Q<Label>(REWARDS_TITLE_LABEL_UI_NAME);
            _rewardsContentContainer = _root.Q<VisualElement>(REWARDS_CONTENT_CONTAINER_UI_NAME);
            _rewardsQuestionMarkLabel = _root.Q<VisualElement>(REWARDS_QUESTION_MARK_LABEL_UI_NAME);

            _itemRewardsListUIController = new ItemRewardsListUIController(
                _rewardsContentContainer,
                itemSlotTemplate,
                REWARD_ITEM_SLOT_CONTAINER_CLASSNAME,
                rewardItemDetailsOverlayTemplate,
                statContainerTemplate
            );
        }

        public void SetQuestRewards(AQuestSO quest)
        {
            _rewardsTitleLabel.text = quest.IsCompleted ? "Earned rewards" : "Rewards";

            bool rewardsCanBeShown = quest.RewardConfiguration.IsRewardShownFromStart || quest.IsCompleted;
            _rewardsQuestionMarkLabel.style.display = rewardsCanBeShown ? DisplayStyle.None : DisplayStyle.Flex;
            _rewardsContentContainer.style.display = rewardsCanBeShown ? DisplayStyle.Flex : DisplayStyle.None;

            if (rewardsCanBeShown)
            {
                _itemRewardsListUIController.UpdateItemRewardsList(
                    quest.IsCompleted ?
                        quest.EarnedReward :
                        quest.RewardConfiguration.FixedRewardConfiguration
                );
            }
        }
    }
}