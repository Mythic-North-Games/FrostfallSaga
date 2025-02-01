using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Utils;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.GameState.City;
using FrostfallSaga.Core.GameState.Dungeon;

namespace FrostfallSaga.Core.GameState
{
    public class GameStateManager : MonoBehaviourPersistingSingleton<GameStateManager>
    {
        private KingdomState _kingdomState;
        private PreFightData _preFightData;
        private PostFightData _postFightData;
        private CityLoadData _cityLoadData;
        private DungeonState _dungeonState;

        protected override void Init()
        {
            _kingdomState = new KingdomState();
            _preFightData = new PreFightData();
            _postFightData = new PostFightData();
            _cityLoadData = new CityLoadData();
            _dungeonState = new DungeonState();
        }

        #region General states

        public bool IsFirstSceneLaunch()
        {
            return _kingdomState.heroGroupData == null;
        }

        #endregion

        #region Kingdom states

        public void SaveKingdomState(
            EntitiesGroupData heroGroupData,
            EntitiesGroupData[] enemiesGroupsData,
            InterestPointData[] interestPointsData
        )
        {
            _kingdomState.heroGroupData = heroGroupData;
            _kingdomState.enemiesGroupsData = enemiesGroupsData;
            _kingdomState.interestPointsData = interestPointsData;
        }

        public KingdomState GetKingdomState()
        {
            return _kingdomState;
        }

        #endregion

        #region Fight states

        public PreFightData GetPreFightData()
        {
            return _preFightData;
        }

        public void SavePreFightData(
            KeyValuePair<string, EntityConfigurationSO>[] alliesEntityConf,
            KeyValuePair<string, EntityConfigurationSO>[] enemiesEntityConf,
            EFightOrigin fightOrigin
        )
        {
            _preFightData.alliesEntityConf = alliesEntityConf;
            _preFightData.enemiesEntityConf = enemiesEntityConf;
            _preFightData.fightOrigin = fightOrigin;
        }

        public void CleanPreFightData()
        {
            _preFightData.alliesEntityConf = null;
            _preFightData.enemiesEntityConf = null;
        }

        public PostFightData GetPostFightData()
        {
            return _postFightData;
        }

        public void SavePostFightData(AFighter[] allies, AFighter[] enemies)
        {
            _postFightData.alliesState = new();
            allies.ToList().ForEach(ally => _postFightData.alliesState.Add(new(ally.EntitySessionId, new(ally.GetHealth()))));

            _postFightData.enemiesState = new();
            enemies.ToList().ForEach(enemy => _postFightData.enemiesState.Add(new(enemy.EntitySessionId, new(enemy.GetHealth()))));

            _postFightData.isActive = true;
        }

        public void CleanPostFightData()
        {
            _postFightData.alliesState = null;
            _postFightData.enemiesState = null;
            _postFightData.isActive = false;
        }

        public bool HasFightJustOccured()
        {
            return _postFightData.isActive;
        }

        public EFightOrigin GetCurrentFightOrigin()
        {
            return _preFightData.fightOrigin;
        }

        #endregion Fight states
    
        #region City states

        public CityLoadData GetCityLoadData()
        {
            return _cityLoadData;
        }

        public InCityConfigurationSO GetCityConfigurationToLoad()
        {
            return _cityLoadData.cityConfigurationToLoad;
        }

        public void SaveCityLoadData(InCityConfigurationSO cityConfiguration)
        {
            _cityLoadData.cityConfigurationToLoad = cityConfiguration;
        }

        #endregion

        #region Dungeon states

        public DungeonState GetDungeonState()
        {
            return _dungeonState;
        }

        public void InitDungeonState(DungeonConfigurationSO dungeonConfiguration)
        {
            _dungeonState.Init(dungeonConfiguration);
        }

        public void SaveDungeonProgress(bool alliesWonLastFight)
        {
            _dungeonState.SaveProgress(alliesWonLastFight);
        }

        public void CleanDungeonState()
        {
            _dungeonState.Reset();
        }

        #endregion
    }
}