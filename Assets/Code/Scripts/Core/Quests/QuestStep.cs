using System;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    [Serializable]
    public class QuestStep
    {
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public QuestStepActions Actions { get; private set; }

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
    }
}