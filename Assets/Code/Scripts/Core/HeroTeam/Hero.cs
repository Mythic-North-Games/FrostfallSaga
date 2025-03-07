using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using UnityEngine;

namespace FrostfallSaga.Core.HeroTeam
{
    public class Hero
    {
        public Hero(string entityConfigurationSOPath)
        {
            EntityConfiguration = Resources.Load<EntityConfigurationSO>(entityConfigurationSOPath);
            PersistedFighterConfiguration = (PersistedFighterConfigurationSO)EntityConfiguration.FighterConfiguration;
        }

        public EntityConfigurationSO EntityConfiguration { get; }
        public PersistedFighterConfigurationSO PersistedFighterConfiguration { get; }

        public bool IsDead()
        {
            return PersistedFighterConfiguration.Health == 0;
        }

        public void FullHeal()
        {
            PersistedFighterConfiguration.SetHealth(PersistedFighterConfiguration.MaxHealth);
        }
    }
}