using System;
using FrostfallSaga.Core.Rewards;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    public abstract class AQuestSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string TeaserDescription { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public string OriginLocation { get; protected set; }
        [field: SerializeField] public Sprite Illustration { get; private set; }
        [field: SerializeField] public EQuestType Type { get; protected set; }
        [field: SerializeField] public RewardConfiguration RewardConfiguration { get; protected set; }
        [field: SerializeField] public Reward EarnedReward { get; protected set; }
        [field: SerializeField] public bool IsCompleted { get; protected set; }
        public bool IsTracked;

        public Action<AQuestSO> onQuestCompleted;

        public abstract void Initialize(MonoBehaviour currentSceneManager);

        protected void CompleteQuest()
        {
            // Mark the quest as completed
            IsCompleted = true;

            // Collect reward (priority to fixed reward)
            EarnedReward = RewardConfiguration.GenerateReward();
            HeroTeam.HeroTeam.Instance.CollectReward(EarnedReward);

            onQuestCompleted?.Invoke(this);
        }
    }
}