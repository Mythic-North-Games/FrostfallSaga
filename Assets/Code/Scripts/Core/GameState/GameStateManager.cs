using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Utils;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Core.GameState.Fight;

namespace FrostfallSaga.Core.GameState
{
    public class GameStateManager : MonoBehaviourPersistingSingleton<GameStateManager>
    {
        private KingdomState _kingdomState;
        private PreFightData _preFightData;
        private PostFightData _postFightData;

        protected override void Init()
        {
            _kingdomState = new KingdomState();
            _preFightData = new PreFightData();
            _postFightData = new PostFightData();
        }

        #region Kingdom states

        public void SaveKingdomState(EntitiesGroupData heroGroupData, EntitiesGroupData[] enemiesGroupsData)
        {
            _kingdomState.heroGroupData = heroGroupData;
            _kingdomState.enemiesGroupsData = enemiesGroupsData;
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
            KeyValuePair<string, EntityConfigurationSO>[] enemiesEntityConf
        )
        {
            _preFightData.alliesEntityConf = alliesEntityConf;
            _preFightData.enemiesEntityConf = enemiesEntityConf;
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

        #endregion Fight states
    }
}