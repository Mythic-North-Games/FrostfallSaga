using System;
using FrostfallSaga.Core.Entities;

namespace FrostfallSaga.Core.GameState.Kingdom
{
    [Serializable]
    public class EntityData
    {
        public EntityConfigurationSO entityConfiguration;
        public string sessionId;
        public bool isDead;
    }
}
