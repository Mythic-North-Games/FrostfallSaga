using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Core.HeroTeam
{
    public class Hero
    {
        public Hero(EntityConfigurationSO entityConfiguration)
        {
            EntityConfiguration = entityConfiguration;
            PersistedFighterConfiguration = (PersistedFighterConfigurationSO)EntityConfiguration.FighterConfiguration;
        }

        public EntityConfigurationSO EntityConfiguration { get; private set; }
        public PersistedFighterConfigurationSO PersistedFighterConfiguration { get; private set; }

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