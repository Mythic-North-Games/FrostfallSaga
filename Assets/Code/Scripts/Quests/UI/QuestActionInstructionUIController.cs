using FrostfallSaga.Core.Quests;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public static class QuestActionInstructionUIController
    {
        /// <summary>
        /// Build the visual element representing an action instruction.
        /// </summary>
        public static VisualElement BuildActionInstruction(
            VisualTreeAsset actionInstructionTemplate,
            AQuestAction action
        )
        {
            VisualElement instructionRoot = actionInstructionTemplate.Instantiate();
            instructionRoot.AddToClassList(ACTION_INSTRUCTION_CONTAINER_CLASSNAME);

            VisualElement completionIcon = instructionRoot.Q<VisualElement>(INSTRUCTION_COMPLETE_ICON_UI_NAME);
            completionIcon.AddToClassList(action.IsCompleted
                ? INSTRUCTION_COMPLETED_ICON_CLASSNAME
                : INSTRUCTION_UNFINISHED_ICON_CLASSNAME);

            Label instructionLabel = instructionRoot.Q<Label>(INSTRUCTION_LABEL_UI_NAME);
            instructionLabel.text = action.IsCompleted ? $"<s>{action.GetInstruction()}</s>" : action.GetInstruction();
            return instructionRoot;
        }

        #region UI Elements Names & Classes

        private static readonly string INSTRUCTION_COMPLETE_ICON_UI_NAME = "InstructionCompletionIcon";
        private static readonly string INSTRUCTION_LABEL_UI_NAME = "InstructionLabel";

        private static readonly string ACTION_INSTRUCTION_CONTAINER_CLASSNAME = "actionInstructionContainer";
        private static readonly string INSTRUCTION_COMPLETED_ICON_CLASSNAME = "instructionCompletionIconCompleted";
        private static readonly string INSTRUCTION_UNFINISHED_ICON_CLASSNAME = "instructionCompletionIconUnfinished";

        #endregion
    }
}