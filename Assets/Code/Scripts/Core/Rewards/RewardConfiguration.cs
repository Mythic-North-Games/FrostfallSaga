using System;
using UnityEngine;

namespace FrostfallSaga.Core.Rewards
{
    [Serializable]
    public class RewardConfiguration
    {
        [field: SerializeField] public Reward FixedRewardConfiguration { get; protected set; }
        [field: SerializeField] public VariableRewardConfiguration VariableRewardConfiguration { get; protected set; }
        [field: SerializeField] public bool IsRewardShownFromStart { get; protected set; }

        public Reward GenerateReward()
        {
            if (FixedRewardConfiguration != null)
            {
                return FixedRewardConfiguration;
            }
            else
            {
                return new Reward
                {
                    StycasEarned = VariableRewardConfiguration.GenerateStycasReward(),
                    ItemsEarned = VariableRewardConfiguration.GenerateItemsReward()
                };
            }
        }
    }
}