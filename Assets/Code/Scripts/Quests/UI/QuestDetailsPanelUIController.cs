using System.Collections;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.DataStructures.TreeNode;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class QuestDetailsPanelUIController
    {
        private static readonly float ELEMENT_TRANSITION_DELAY = 0.1f;

        #region UI Elements Names & Classes
        private static readonly string QUEST_DETAILS_PANEL_CONTAINER_UI_NAME = "QuestDetailsPanelContainer";
        private static readonly string QUEST_DETAILS_HEADER_UI_NAME = "QuestDetailsHeader";
        private static readonly string QUEST_TITLE_UI_NAME = "Title";
        private static readonly string QUEST_DESCRIPTION_UI_NAME = "Description";
        private static readonly string QUEST_ILLUSTRATION_BACKGROUND_UI_NAME = "QuestIllustrationBackground";
        private static readonly string QUEST_REWARDS_CONTAINER_UI_NAME = "RewardsContainer";
        private static readonly string QUEST_CONCLUSION_LABEL_UI_NAME = "ConclusionLabel";
        private static readonly string QUEST_ACTIONS_CONTAINER_UI_NAME = "QuestActionsContainer";
        private static readonly string QUEST_ACTIONS_INSTRUCTIONS_CONTAINER_UI_NAME = "QuestActionsInstructionsList";
        private static readonly string QUEST_STEPS_CONTAINER_UI_NAME = "QuestStepsContainer";

        private static readonly string QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME = "questDetailsPanelContainerHidden";
        private static readonly string ILLUSTRATION_BACKGROUND_HIDDEN_CLASSNAME = "questIllustrationBackgroundHidden";
        private static readonly string QUEST_DETAILS_HEADER_HIDDEN_CLASSNAME = "questDetailsHeaderHidden";
        private static readonly string QUEST_REWARDS_CONTAINER_HIDDEN_CLASSNAME = "questRewardsContainerHidden";
        private static readonly string QUEST_CONCLUSION_LABEL_HIDDEN_CLASSNAME = "questConclusionLabelHidden";
        private static readonly string QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME = "questStepsContainerHidden";
        #endregion

        private readonly VisualElement _container;
        private readonly VisualElement _illustrationBackground;
        private readonly VisualElement _questDetailsHeader;
        private readonly Label _titleLabel;
        private readonly Label _descriptionLabel;
        private readonly VisualElement _questRewardsContainer;
        private readonly Label _conclusionLabel;
        private readonly VisualElement _questActionsContainer;
        private readonly VisualElement _questActionsInstructionsContainer;
        private readonly VisualElement _questStepsContainer;
        private readonly VisualTreeAsset _questStepTemplate;
        private readonly VisualTreeAsset _actionInstructionTemplate;
        private readonly VisualTreeAsset _actionInstructionSeparatorTemplate;
        private readonly QuestRewardsUIController _questRewardsUIController;

        private AQuestSO _currentQuest;

        public QuestDetailsPanelUIController(
            VisualElement root,
            VisualTreeAsset questStepTemplate,
            VisualTreeAsset actionInstructionTemplate,
            VisualTreeAsset actionInstructionSeparatorTemplate,
            VisualTreeAsset itemSlotTemplate,
            VisualTreeAsset rewardItemDetailsOverlay,
            VisualTreeAsset statContainerTemplate
        )
        {
            _container = root.Q<VisualElement>(QUEST_DETAILS_PANEL_CONTAINER_UI_NAME);
            _illustrationBackground = root.Q<VisualElement>(QUEST_ILLUSTRATION_BACKGROUND_UI_NAME);
            _questDetailsHeader = root.Q<VisualElement>(QUEST_DETAILS_HEADER_UI_NAME);
            _titleLabel = root.Q<Label>(QUEST_TITLE_UI_NAME);
            _descriptionLabel = root.Q<Label>(QUEST_DESCRIPTION_UI_NAME);
            _questRewardsContainer = root.Q<VisualElement>(QUEST_REWARDS_CONTAINER_UI_NAME);
            _conclusionLabel = root.Q<Label>(QUEST_CONCLUSION_LABEL_UI_NAME);
            _questActionsContainer = root.Q<VisualElement>(QUEST_ACTIONS_CONTAINER_UI_NAME);
            _questActionsInstructionsContainer = root.Q<VisualElement>(QUEST_ACTIONS_INSTRUCTIONS_CONTAINER_UI_NAME);
            _questStepsContainer = root.Q<VisualElement>(QUEST_STEPS_CONTAINER_UI_NAME);

            _questStepTemplate = questStepTemplate;
            _actionInstructionTemplate = actionInstructionTemplate;
            _actionInstructionSeparatorTemplate = actionInstructionSeparatorTemplate;

            _questRewardsUIController = new QuestRewardsUIController(
                _questRewardsContainer,
                itemSlotTemplate,
                rewardItemDetailsOverlay,
                statContainerTemplate
            );

            // Hide all the parts of the panel
            _container.AddToClassList(QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            _illustrationBackground.AddToClassList(ILLUSTRATION_BACKGROUND_HIDDEN_CLASSNAME);
            _questDetailsHeader.AddToClassList(QUEST_DETAILS_HEADER_HIDDEN_CLASSNAME);
            _questRewardsContainer.AddToClassList(QUEST_REWARDS_CONTAINER_HIDDEN_CLASSNAME);
            _conclusionLabel.AddToClassList(QUEST_CONCLUSION_LABEL_HIDDEN_CLASSNAME);
            _questActionsContainer.AddToClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
            _questStepsContainer.AddToClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
        }

        /// <summary>
        /// Display the details of the given quest in the panel.
        /// </summary>
        /// <param name="quest">The quest to display.</param>
        public IEnumerator DisplayQuestDetails(AQuestSO quest)
        {
            _currentQuest = quest;

            // First setup all the UI elements
            SetupQuestHeader(quest);
            _questRewardsUIController.SetQuestRewards(quest);

            if (quest is ActionsQuestSO actionsQuest)
            {
                _conclusionLabel.text = actionsQuest.ConclusionText;
                SetupActionsQuest(actionsQuest);
            }
            else if (quest is StepsQuestSO stepsQuest)
            {
                _conclusionLabel.text = stepsQuest.ChosenQuestEnding.ConclusionText;
                SetupStepsQuest(stepsQuest);
            }

            // Then show the details panel part per part

            // 1. Show the illustration background, the main container and the header
            _container.RemoveFromClassList(QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            _illustrationBackground.RemoveFromClassList(ILLUSTRATION_BACKGROUND_HIDDEN_CLASSNAME);
            _questDetailsHeader.RemoveFromClassList(QUEST_DETAILS_HEADER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);

            // 2. Show the rewards container
            _questRewardsContainer.RemoveFromClassList(QUEST_REWARDS_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);

            // 3. Show the conclusion label if needed
            if (quest.IsCompleted)
            {
                _conclusionLabel.RemoveFromClassList(QUEST_CONCLUSION_LABEL_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);
            }

            // 5. Show the actions or steps containers
            if (quest is ActionsQuestSO) _questActionsContainer.RemoveFromClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
            else _questStepsContainer.RemoveFromClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
        }

        /// <summary>
        /// Reset the details panel to its inital state (no data) and hide it.
        /// </summary>
        public IEnumerator ResetAndHidePanel()
        {
            // 1. Hide the actions and steps containers
            if (_currentQuest is ActionsQuestSO) _questActionsContainer.AddToClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
            else _questStepsContainer.AddToClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);
            _questActionsInstructionsContainer.Clear();
            _questStepsContainer.Clear();

            // 2. Hide the conclusion label if needed
            if (_currentQuest.IsCompleted)
            {
                _conclusionLabel.AddToClassList(QUEST_CONCLUSION_LABEL_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);
                _conclusionLabel.text = string.Empty;
            }

            // 3. Hide the rewards container
            _questRewardsContainer.AddToClassList(QUEST_REWARDS_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);

            // 4. Hide the quest details header, the main container and the background illustration
            _questDetailsHeader.AddToClassList(QUEST_DETAILS_HEADER_HIDDEN_CLASSNAME);
            _container.AddToClassList(QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            _illustrationBackground.AddToClassList(ILLUSTRATION_BACKGROUND_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(ELEMENT_TRANSITION_DELAY);
            _titleLabel.text = string.Empty;
            _descriptionLabel.text = string.Empty;
            _illustrationBackground.style.backgroundImage = new StyleBackground();
        }

        public bool IsDisplayed()
        {
            return !_container.ClassListContains(QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
        }

        private void SetupQuestHeader(AQuestSO quest)
        {
            _titleLabel.text = quest.Name;
            _descriptionLabel.text = quest.Description;
            _illustrationBackground.style.backgroundImage = new(quest.Illustration);
        }

        private void SetupActionsQuest(ActionsQuestSO actionsQuest)
        {
            _questActionsInstructionsContainer.Clear();

            foreach (AQuestAction action in actionsQuest.actions)
            {
                VisualElement actionInstruction = QuestActionInstructionUIController.BuildActionInstruction(
                    _actionInstructionTemplate, action
                );
                _questActionsInstructionsContainer.Add(actionInstruction);
            }
        }

        private void SetupStepsQuest(StepsQuestSO stepsQuest)
        {
            _questStepsContainer.Clear();
            BuildQuestStepContainers(stepsQuest.Steps);
        }

        private void BuildQuestStepContainers(TreeNode<QuestStep> step)
        {
            QuestStep currentStep = step.Data;

            // If completed, show the next steps in the tree first (latest unfinished steps displayed first)
            if (currentStep.IsCompleted() && step.HasChildren())
            {
                int nextStepIndex = currentStep.Actions.ChosenDecisiveActionIndex;
                TreeNode<QuestStep> nextStep = step.Children[nextStepIndex];
                BuildQuestStepContainers(nextStep);
            }
            
            // After that, show the current step
            VisualElement questStepContainer = QuestStepContainerUIController.BuildQuestStepContainer(
                _questStepTemplate,
                _actionInstructionTemplate,
                _actionInstructionSeparatorTemplate,
                currentStep
            );
            _questStepsContainer.Add(questStepContainer);
        }
    }
}