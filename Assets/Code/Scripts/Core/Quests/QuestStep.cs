using System;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    [Serializable]
    public class QuestStep
    {
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public QuestStepActionsSO Actions { get; private set; }

        public Action<QuestStep> onQuestStepCompleted;

        public MonoBehaviour CurrentSceneManager { get; private set; }

        public void Initialize(MonoBehaviour currentSceneManager)
        {
            CurrentSceneManager = currentSceneManager;
            Actions.Initialize(currentSceneManager);
            Actions.onStepActionsCompleted += () => onQuestStepCompleted?.Invoke(this);
        }

        public bool IsCompleted()
        {
            return Actions.ChosenDecisiveActionIndex > -1;
        }

        #if UNITY_EDITOR

        public QuestStep(string title, string description, QuestStepActionsSO actions)
        {
            Title = title;
            Description = description;
            Actions = actions;
        }

        public void SetTitle(string newTitle)
        {
            Title = newTitle;
        }

        public void SetDescription(string newDescription)
        {
            Description = newDescription;
        }

        public void SetActions(QuestStepActionsSO newActions)
        {
            Actions = newActions;
        }

        #endif
    }
}