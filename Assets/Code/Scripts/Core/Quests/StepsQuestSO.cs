using System.Collections.Generic;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Trees;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    [CreateAssetMenu(fileName = "StepsQuest", menuName = "ScriptableObjects/Quests/StepsQuest", order = 0)]
    public class StepsQuestSO : AQuestSO
    {
        [field: SerializeField]
        [field: Tooltip("Possible endings depending on last quest step decisive action.")]
        public SElementToValue<int[], QuestEnding>[] PossibleQuestEndings { get; private set; }

        [field: SerializeField] public TreeNode<QuestStep> Steps { get; private set; }

        public QuestEnding ChosenQuestEnding { get; private set; }


        /// <summary>
        ///     Start listening to the events that will update the action completion.
        /// </summary>
        /// <param name="currentSceneManager">The specific manager of the scene the action is related to.</param>
        public override void Initialize(MonoBehaviour currentSceneManager)
        {
            if (IsCompleted)
            {
                Debug.LogWarning("The quest is already completed. No need to update the completion.");
                return;
            }

            InitializeQuestSteps(Steps, currentSceneManager);
        }

        private void InitializeQuestSteps(TreeNode<QuestStep> questStep, MonoBehaviour currentSceneManager)
        {
            // If the quest step is not completed, initialize it but not the children yet.
            if (!questStep.GetData().IsCompleted())
            {
                questStep.GetData().Initialize(currentSceneManager);
                questStep.GetData().onQuestStepCompleted += OnQuestStepCompleted;
                return;
            }

            // If the quest step is completed, initialize the next child step.
            int decisiveActionIndex = questStep.GetData().Actions.ChosenDecisiveActionIndex;
            TreeNode<QuestStep> nextStep = questStep.GetChildren()[decisiveActionIndex];
            InitializeQuestSteps(nextStep, currentSceneManager);
        }

        private void OnQuestStepCompleted(QuestStep completedStep)
        {
            // Unsubscribe from the event to avoid multiple completion of the same step.
            completedStep.onQuestStepCompleted -= OnQuestStepCompleted;

            // Try to find the last completed step path.
            int[] lastCompletedStepPath = GetLastCompletedStepPath(Steps);

            // If no completed step path, the quest is not completed yet. Initialize the next step.
            if (lastCompletedStepPath.Length == 0)
            {
                InitializeQuestSteps(TreeNode<QuestStep>.FindChild(Steps, completedStep),
                    completedStep.CurrentSceneManager);
                return;
            }

            // Otherwise, the quest is completed. Set the chosen quest ending and complete the quest.
            ChosenQuestEnding =
                SElementToValue<int[], QuestEnding>.GetDictionaryFromArray(PossibleQuestEndings)[lastCompletedStepPath];
            CompleteQuest();
        }

        private int[] GetLastCompletedStepPath(TreeNode<QuestStep> questStep, TreeNode<QuestStep> parent = null,
            List<int> lastCompletedStepPath = null)
        {
            if (!questStep.GetData().IsCompleted()) return new int[0];

            lastCompletedStepPath ??= new List<int>();
            lastCompletedStepPath.Add(parent == null ? 0 : parent.GetChildren().IndexOf(questStep));

            if (questStep.GetChildren().Count == 0) return lastCompletedStepPath.ToArray();

            TreeNode<QuestStep> nextCompletedStep = questStep.GetChildren().Find(
                childStep => childStep.GetData().IsCompleted()
            );
            if (nextCompletedStep == null) return new int[0];

            return GetLastCompletedStepPath(nextCompletedStep, questStep, lastCompletedStepPath);
        }

#if UNITY_EDITOR

        public void SetSteps(TreeNode<QuestStep> newSteps)
        {
            Steps = newSteps;
        }

        public void SetPossibleQuestEndings(SElementToValue<int[], QuestEnding>[] newPossibleQuestEndings)
        {
            PossibleQuestEndings = newPossibleQuestEndings;
        }

#endif
    }
}