using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;

namespace FrostfallSaga.Core.GameState.Fight
{
    [Serializable]
    public class PreFightData
    {
        public EntityConfigurationSO[] alliesEntityConf;
        public EFightOrigin fightOrigin;
        public KeyValuePair<string, EntityConfigurationSO>[] enemiesEntityConf;
    }
}