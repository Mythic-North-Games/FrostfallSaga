using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.DataStructures.TreeNode;

namespace FrostfallSaga.Core.Quests
{
    [CreateAssetMenu(fileName = "StepsQuest", menuName = "ScriptableObjects/Quests/StepsQuest", order = 0)]
    public class StepsQuestSO : AQuestSO
    {
        [field: SerializeField]
        [field: Tooltip("Possible endings depending on last quest step decisive action.")]
        private SElementToValue<int[], QuestEnding>[] _possibleQuestEndings;
        public Dictionary<int[], QuestEnding> PossibleQuestEndings
        {
            get => SElementToValue<int[], QuestEnding>.GetDictionaryFromArray(_possibleQuestEndings);
            private set => _possibleQuestEndings = SElementToValue<int[], QuestEnding>.GetArrayFromDictionary(value);
        }

        [field: SerializeField]
        [field: Tooltip("Only set this in the editor for testing. Will be overriden at runtime if the quest is completed for real.")]
        public QuestEnding ChosenQuestEnding { get; private set; }

        ///////////////////::///
        /// Quest steps tree ///
        //////////////////:::///
        public TreeNode<QuestStep> Steps => _runtimeSteps;
        private TreeNode<QuestStep> _runtimeSteps;
        [SerializeField] private List<TreeNodeDTO<QuestStep>> _serializedSteps;

        private void OnEnable()
        {
            _runtimeSteps = TreeNodeSerializer.DeserializeTree(_serializedSteps);
        }

#if UNITY_EDITOR
        public void SetSteps(TreeNode<QuestStep> newSteps)
        {
            _runtimeSteps = newSteps;
            SaveSteps();
        }

        public void SaveSteps()
        {
            _serializedSteps = TreeNodeSerializer.SerializeTree(_runtimeSteps);
        }

        public void SetPossibleQuestEndings(Dictionary<int[], QuestEnding> newPossibleQuestEndings)
        {
            PossibleQuestEndings = newPossibleQuestEndings;
        }
#endif

        public override void Initialize(MonoBehaviour currentSceneManager)
        {
            if (IsCompleted)
            {
                Debug.LogWarning("The quest is already completed. No need to update the completion.");
                return;
            }

            InitializeQuestSteps(_runtimeSteps, currentSceneManager);
        }

        private void InitializeQuestSteps(TreeNode<QuestStep> questStep, MonoBehaviour currentSceneManager)
        {
            if (!questStep.Data.IsCompleted())
            {
                questStep.Data.Initialize(currentSceneManager);
                questStep.Data.onQuestStepCompleted += OnQuestStepCompleted;
                return;
            }

            int decisiveActionIndex = questStep.Data.Actions.ChosenDecisiveActionIndex;
            TreeNode<QuestStep> nextStep = questStep.Children[decisiveActionIndex];
            InitializeQuestSteps(nextStep, currentSceneManager);
        }

        private void OnQuestStepCompleted(QuestStep completedStep)
        {
            completedStep.onQuestStepCompleted -= OnQuestStepCompleted;

            int[] lastCompletedStepPath = GetLastCompletedStepPath(_runtimeSteps);
            if (lastCompletedStepPath.Length == 0)
            {
                InitializeQuestSteps(TreeNode<QuestStep>.FindChild(_runtimeSteps, completedStep), completedStep.CurrentSceneManager);
                return;
            }

            ChosenQuestEnding = PossibleQuestEndings[lastCompletedStepPath];
            CompleteQuest();
        }

        private int[] GetLastCompletedStepPath(TreeNode<QuestStep> questStep, TreeNode<QuestStep> parent = null, List<int> lastCompletedStepPath = null)
        {
            if (!questStep.Data.IsCompleted()) return new int[0];

            lastCompletedStepPath ??= new List<int>();
            lastCompletedStepPath.Add(parent == null ? 0 : parent.Children.IndexOf(questStep));

            if (questStep.Children.Count == 0) return lastCompletedStepPath.ToArray();

            TreeNode<QuestStep> nextCompletedStep = questStep.Children.Find(childStep => childStep.Data.IsCompleted());
            if (nextCompletedStep == null) return new int[0];

            return GetLastCompletedStepPath(nextCompletedStep, questStep, lastCompletedStepPath);
        }
    }
}
