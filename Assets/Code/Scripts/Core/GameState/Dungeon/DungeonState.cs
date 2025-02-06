using FrostfallSaga.Core.Dungeons;

namespace FrostfallSaga.Core.GameState.Dungeon
{
    public class DungeonState
    {
        public DungeonConfigurationSO DungeonConfiguration { get; private set; }
        public int CurrentDungeonFightIndex { get; private set; }
        public bool AlliesWonLastFight { get; private set; }

        public void Init(DungeonConfigurationSO dungeonConfiguration)
        {
            DungeonConfiguration = dungeonConfiguration;
            CurrentDungeonFightIndex = 0;
            AlliesWonLastFight = true;
        }
        
        public bool IsDungeonCompleted()
        {
            return CurrentDungeonFightIndex == DungeonConfiguration.PreBossFightConfigurations.Length + 1;
        }

        public bool IsAtBossFight()
        {
            return CurrentDungeonFightIndex == DungeonConfiguration.PreBossFightConfigurations.Length;
        }

        public void SaveProgress(bool alliesWonLastFight)
        {
            AlliesWonLastFight = alliesWonLastFight;
            if (alliesWonLastFight) CurrentDungeonFightIndex++;
        }

        public void Reset()
        {
            DungeonConfiguration = null;
            CurrentDungeonFightIndex = 0;
            AlliesWonLastFight = false;
        }
    }
}
