using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;

namespace FrostfallSaga.Core.GameState.Fight
{
    [Serializable]
    public class PreFightData
    {
        public EntityConfigurationSO[] alliesEntityConf;
        public KeyValuePair<string, EntityConfigurationSO>[] enemiesEntityConf;
        public EFightOrigin fightOrigin;
    }
}
