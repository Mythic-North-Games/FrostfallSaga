using UnityEngine.UIElements;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Trees;

namespace FrostfallSaga.Quests.UI
{
    public static class QuestDetailsPanelUIController
    {
        #region UI Elements Names & Classes
        private static readonly string QUEST_DETAILS_PANEL_CONTAINER_UI_NAME = "QuestDetailsPanelContainer";
        private static readonly string QUEST_TITLE_UI_NAME = "Title";
        private static readonly string QUEST_ORIGIN_LOCATION_UI_NAME = "OriginLocation";
        private static readonly string QUEST_DESCRIPTION_UI_NAME = "Description";
        private static readonly string QUEST_ACTIONS_CONTAINER_UI_NAME = "QuestActionsContainer";
        private static readonly string QUEST_STEPS_CONTAINER_UI_NAME = "QuestStepsContainer";

        private static readonly string QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME = "questDetailsPanelContainerHidden";
        private static readonly string QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME = "questStepsContainerHidden";
        #endregion

        /// <summary>
        /// Display the details of the given quest in the given panel.
        /// </summary>
        /// <param name="questDetailsPanelRoot">The root of the quest details panel.</param>
        /// <param name="questStepTemplate">The template of a quest step.</param>
        /// <param name="actionInstructionTemplate">The template of an action instruction.</param>
        /// <param name="quest">The quest to display.</param>
        public static void DisplayQuestDetails(
            VisualElement questDetailsPanelRoot,
            VisualTreeAsset questStepTemplate,
            VisualTreeAsset actionInstructionTemplate,
            AQuestSO quest
        )
        {
            ResetQuestDetailsPanel(questDetailsPanelRoot);
            SetupQuestHeader(questDetailsPanelRoot, quest);

            if (quest is ActionsQuestSO actionsQuest)
            {
                SetupActionsQuest(questDetailsPanelRoot, actionInstructionTemplate, actionsQuest);
            }
            else if (quest is StepsQuestSO stepsQuest)
            {
                SetupStepsQuest(
                    questDetailsPanelRoot,
                    questStepTemplate,
                    actionInstructionTemplate,
                    stepsQuest
                );
            }

            questDetailsPanelRoot.Q<VisualElement>(
                QUEST_DETAILS_PANEL_CONTAINER_UI_NAME
            ).RemoveFromClassList(QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
        }

        public static void ResetQuestDetailsPanel(VisualElement questDetailsPanelRoot)
        {
            questDetailsPanelRoot.Q<Label>(QUEST_TITLE_UI_NAME).text = string.Empty;
            questDetailsPanelRoot.Q<Label>(QUEST_ORIGIN_LOCATION_UI_NAME).text = string.Empty;
            questDetailsPanelRoot.Q<Label>(QUEST_DESCRIPTION_UI_NAME).text = string.Empty;

            questDetailsPanelRoot.Q<VisualElement>(
                QUEST_DETAILS_PANEL_CONTAINER_UI_NAME
            ).AddToClassList(QUEST_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            questDetailsPanelRoot.Q<VisualElement>(QUEST_ACTIONS_CONTAINER_UI_NAME).AddToClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
            questDetailsPanelRoot.Q<VisualElement>(QUEST_STEPS_CONTAINER_UI_NAME).AddToClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
        }

        private static void SetupQuestHeader(VisualElement questDetailsPanelRoot, AQuestSO quest)
        {
            questDetailsPanelRoot.Q<Label>(QUEST_TITLE_UI_NAME).text = quest.Name;
            questDetailsPanelRoot.Q<Label>(QUEST_ORIGIN_LOCATION_UI_NAME).text = quest.OriginLocation;
            questDetailsPanelRoot.Q<Label>(QUEST_DESCRIPTION_UI_NAME).text = quest.Description;
        }

        private static void SetupActionsQuest(
            VisualElement questDetailsPanel,
            VisualTreeAsset actionInstructionTemplate,
            ActionsQuestSO actionsQuest
        )
        {
            VisualElement questActionsContainer = questDetailsPanel.Q<VisualElement>(QUEST_ACTIONS_CONTAINER_UI_NAME);
            questActionsContainer.Clear();

            foreach (AQuestAction action in actionsQuest.Actions)
            {
                VisualElement actionInstruction = QuestActionInstructionUIController.BuildActionInstruction(
                    actionInstructionTemplate, action
                );
                questActionsContainer.Add(actionInstruction);
            }

            questActionsContainer.RemoveFromClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
        }

        private static void SetupStepsQuest(
            VisualElement questDetailsPanel,
            VisualTreeAsset questStepTemplate,
            VisualTreeAsset actionInstructionTemplate,
            StepsQuestSO stepsQuest
        )
        {
            VisualElement questStepsContainer = questDetailsPanel.Q<VisualElement>(QUEST_STEPS_CONTAINER_UI_NAME);
            questStepsContainer.Clear();

            BuildQuestStepContainers(
                questStepsContainer,
                questStepTemplate,
                actionInstructionTemplate,
                stepsQuest.Steps
            );

            questStepsContainer.RemoveFromClassList(QUEST_STEPS_CONTAINER_HIDDEN_CLASSNAME);
        }

        private static void BuildQuestStepContainers(
            VisualElement questStepsContainer,
            VisualTreeAsset questStepTemplate,
            VisualTreeAsset actionInstructionTemplate,
            TreeNode<QuestStep> step
        )
        {
            QuestStep currentStep = step.GetData();

            // Show the step
            VisualElement questStepContainer = QuestStepContainerUIController.BuildQuestStepContainer(
                questStepTemplate,
                actionInstructionTemplate,
                currentStep,
                !step.HasChildren() || !currentStep.IsCompleted()
            );
            questStepsContainer.Add(questStepContainer);

            // If completed, show the next step in the tree
            if (currentStep.IsCompleted() && step.HasChildren())
            {
                int nextStepIndex = currentStep.Actions.ChosenDecisiveActionIndex;
                TreeNode<QuestStep> nextStep = step.GetChildren()[nextStepIndex];
                BuildQuestStepContainers(questStepsContainer, questStepTemplate, actionInstructionTemplate, nextStep);
            }
        }
    }
}