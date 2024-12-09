using System;
using FrostfallSaga.Core;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.KingdomToFight
{
    [Serializable]
    public class EntityToFighter
    {
        public EEntityID entityID;
        public FighterConfigurationSO fighterConfiguration;
    }
}