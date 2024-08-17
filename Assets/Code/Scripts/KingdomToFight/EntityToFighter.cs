using System;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.KingdomToFight
{
    [Serializable]
    public class EntityToFighter
    {
        public EntityType entityType;
        public FighterConfigurationSO fighterConfiguration;
    }
}