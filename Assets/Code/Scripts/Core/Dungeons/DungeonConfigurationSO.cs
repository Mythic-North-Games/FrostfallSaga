using System;
using UnityEngine;

namespace FrostfallSaga.Core.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonConfiguration", menuName = "ScriptableObjects/Dungeons/DungeonConfiguration", order = 0)]
    public class DungeonConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public DungeonFightConfiguration BossFightConfiguration { get; private set; }
        [field: SerializeField] public DungeonFightConfiguration[] PreBossFightConfigurations { get; private set; }
        [field: SerializeField] public RewardConfiguration RewardConfiguration { get; private set; }

        public Action<DungeonConfigurationSO> onDungeonCompleted;

        public void CompleteDungeon()
        {
            // Reward the hero team
            HeroTeam.HeroTeam.Instance.CollectReward(RewardConfiguration);

            onDungeonCompleted?.Invoke(this);
        }
    }
}