using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.GameState.City;
using FrostfallSaga.Core.GameState.Dungeon;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.GameState.Grid;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using JetBrains.Annotations;

namespace FrostfallSaga.Core.GameState
{
    public class GameStateManager : MonoBehaviourPersistingSingleton<GameStateManager>
    {
        private CityLoadData _cityLoadData;
        private DungeonState _dungeonState;
        private KingdomState _kingdomState;
        private PostFightData _postFightData;
        private PreFightData _preFightData;

        static GameStateManager()
        {
            AutoInitializeOnSceneLoad = true;
        }

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
            KingdomCellData[] kingdomCellsData,
            EntitiesGroupData heroGroupData,
            EntitiesGroupData[] enemiesGroupsData,
            InterestPointData[] interestPointsData
        )
        {
            _kingdomState.kingdomCellsData = kingdomCellsData;
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
            EntityConfigurationSO[] alliesEntityConf,
            KeyValuePair<string, EntityConfigurationSO>[] enemiesEntityConf,
            EFightOrigin fightOrigin,
            [CanBeNull] Dictionary<HexDirection, Cell> hexDirectionCell)
        {
            _preFightData.alliesEntityConf = alliesEntityConf;
            _preFightData.enemiesEntityConf = enemiesEntityConf;
            _preFightData.fightOrigin = fightOrigin;
            if (hexDirectionCell != null)
                _preFightData.HexDirectionCells = hexDirectionCell;
        }

        public void CleanPreFightData()
        {
            _preFightData.alliesEntityConf = null;
            _preFightData.enemiesEntityConf = null;
            _preFightData.HexDirectionCells = null;
        }

        public PostFightData GetPostFightData()
        {
            return _postFightData;
        }

        public void SavePostFightData(AFighter[] enemies)
        {
            _postFightData.enemiesState = new List<SElementToValue<string, PostFightFighterState>>();
            enemies.ToList().ForEach(enemy =>
                _postFightData.enemiesState.Add(
                    new SElementToValue<string, PostFightFighterState>(enemy.EntitySessionId,
                        new PostFightFighterState(enemy.GetHealth()))));

            _postFightData.isActive = true;
        }

        public void CleanPostFightData()
        {
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