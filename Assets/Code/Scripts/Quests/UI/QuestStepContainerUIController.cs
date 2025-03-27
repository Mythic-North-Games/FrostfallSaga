using UnityEngine.UIElements;
using FrostfallSaga.Core.Quests;

namespace FrostfallSaga.Quests.UI
{
    public static class QuestStepContainerUIController
    {
        #region UI Elements Names & Classes
        private static readonly string STEP_CONTAINER_ROOT_UI_NAME = "QuestStepContainer";
        private static readonly string TITLE_UI_NAME = "QuestStepTitle";
        private static readonly string DESCRIPTION_UI_NAME = "QuestStepDescription";
        private static readonly string COMPLETED_ICON_UI_NAME = "QuestStepCompletedIcon";
        private static readonly string ACTIONS_LIST_UI_NAME = "ActionsList";

        private static readonly string STEP_CONTAINER_ROOT_COMPLETED_CLASSNAME = "questStepContainerRootCompleted";
        private static readonly string QUEST_STEP_CONTAINER_CLASSNAME = "questStepContainer";
        private static readonly string QUEST_STEP_CONTAINER_LAST_CLASSNAME = "questStepContainerLast";
        private static readonly string DECISIVE_ACTION_SEPARATOR_CLASSNAME = "decisiveActionSeparator";
        #endregion

        /// <summary>
        /// Build the visual element representing a quest step.
        /// </summary>
        /// <param name="questStepTemplate">The template of a quest step.</param>
        /// <param name="actionInstructionTemplate">The template of an action instruction.</param>
        /// <param name="questStep">The quest step to display.</param>
        public static VisualElement BuildQuestStepContainer(
            VisualTreeAsset questStepTemplate,
            VisualTreeAsset actionInstructionTemplate,
            QuestStep questStep,
            bool isLast = false
        )
        {
            VisualElement questStepRoot = questStepTemplate.Instantiate();
            questStepRoot.AddToClassList(isLast ? QUEST_STEP_CONTAINER_LAST_CLASSNAME : QUEST_STEP_CONTAINER_CLASSNAME);

            if (questStep.IsCompleted())
            {
                questStepRoot.Q<VisualElement>(STEP_CONTAINER_ROOT_UI_NAME).AddToClassList(
                    STEP_CONTAINER_ROOT_COMPLETED_CLASSNAME
                );
            }
            questStepRoot.Q<VisualElement>(COMPLETED_ICON_UI_NAME).visible = questStep.IsCompleted();

            questStepRoot.Q<Label>(TITLE_UI_NAME).text = questStep.Title;
            questStepRoot.Q<Label>(DESCRIPTION_UI_NAME).text = questStep.Description;

            VisualElement actionsList = questStepRoot.Q<VisualElement>(ACTIONS_LIST_UI_NAME);
            actionsList.Clear();

            // Display non decisive actions
            foreach (AQuestAction action in questStep.Actions.NonDecisiveActions)
            {
                VisualElement actionInstruction = QuestActionInstructionUIController.BuildActionInstruction(
                    actionInstructionTemplate,
                    action
                );
                actionsList.Add(actionInstruction);
            }

            // Display the completed decisive action or all decisive actions
            AQuestAction completedDecisiveAction = questStep.Actions.DecisiveActions.Find(
                (action) => action.IsCompleted
            );
            if (completedDecisiveAction != null)
            {
                VisualElement actionInstruction = QuestActionInstructionUIController.BuildActionInstruction(
                    actionInstructionTemplate,
                    completedDecisiveAction
                );
                actionsList.Add(actionInstruction);
            }
            else
            {
                for (int i = 0; i < questStep.Actions.DecisiveActions.Count; i++)
                {
                    AQuestAction action = questStep.Actions.DecisiveActions[i];
                    VisualElement actionInstruction = QuestActionInstructionUIController.BuildActionInstruction(
                        actionInstructionTemplate,
                        action
                    );
                    actionsList.Add(actionInstruction);

                    // Add a separator between decisive actions
                    if (i < questStep.Actions.DecisiveActions.Count - 1)
                    {
                        Label separator = new("---or---");
                        separator.AddToClassList(DECISIVE_ACTION_SEPARATOR_CLASSNAME);
                        actionsList.Add(separator);
                    }
                }
            }

            return questStepRoot;
        }
    }
}