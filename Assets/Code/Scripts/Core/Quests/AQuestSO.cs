using System;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    public abstract class AQuestSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public string OriginLocation { get; protected set; }
        [field: SerializeField] public EQuestType Type { get; protected set; }
        [field: SerializeField] public RewardConfiguration RewardConfiguration { get; protected set; }
        [field: SerializeField] public bool IsCompleted { get; protected set; }

        public Action<AQuestSO> onQuestCompleted;

        public abstract void Initialize(MonoBehaviour currentSceneManager);

        protected void CompleteQuest()
        {
            // Mark the quest as completed
            IsCompleted = true;

            // Reward the hero team
            HeroTeam.HeroTeam.Instance.CollectReward(RewardConfiguration);
            
            onQuestCompleted?.Invoke(this);
        }
    }
}