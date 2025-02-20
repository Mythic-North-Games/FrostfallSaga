using System;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    public abstract class AQuestSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public EQuestType Type { get; protected set; }
        [field: SerializeField] public bool IsCompleted { get; protected set; }

        public Action<AQuestSO> onQuestCompleted;

        public abstract void Initialize(MonoBehaviour currentSceneManager);

        protected void CompleteQuest()
        {
            IsCompleted = true;
            onQuestCompleted?.Invoke(this);
        }
    }
}