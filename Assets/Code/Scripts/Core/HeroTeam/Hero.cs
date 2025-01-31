using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Core.HeroTeam
{
    public class Hero
    {
        private EntityConfigurationSO _entityConfiguration;
        private PersistedFighterConfigurationSO _persistedFighterConfiguration;

        public Hero(string entityConfigurationSOPath)
        {
            _entityConfiguration = Resources.Load<EntityConfigurationSO>(entityConfigurationSOPath);
            _persistedFighterConfiguration = (PersistedFighterConfigurationSO)_entityConfiguration.FighterConfiguration;
        }

        public void FullHeal()
        {
            //_persistedFighterConfiguration.SetHealth(_persistedFighterConfiguration.MaxHealth);
        }
    }
}