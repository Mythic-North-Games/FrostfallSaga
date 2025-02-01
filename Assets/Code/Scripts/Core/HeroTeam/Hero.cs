using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Core.HeroTeam
{
    public class Hero
    {
        public EntityConfigurationSO EntityConfiguration { get; private set; }
        public PersistedFighterConfigurationSO PersistedFighterConfiguration { get; private set; }

        public Hero(string entityConfigurationSOPath)
        {
            EntityConfiguration = Resources.Load<EntityConfigurationSO>(entityConfigurationSOPath);
            PersistedFighterConfiguration = (PersistedFighterConfigurationSO)EntityConfiguration.FighterConfiguration;
        }

        public void FullHeal()
        {
            PersistedFighterConfiguration.SetHealth(PersistedFighterConfiguration.MaxHealth);
        }
    }
}