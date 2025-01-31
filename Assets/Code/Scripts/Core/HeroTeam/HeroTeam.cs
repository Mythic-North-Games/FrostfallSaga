using System.Collections.Generic;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Core.HeroTeam
{
    public class HeroTeam : MonoBehaviourPersistingSingleton<HeroTeam>
    {
        private const string BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH = "ScriptableObjects/Entities/{0}/{1}EntityConfiguration";
        private const string HERO_CONFIG_NAME = "Hero";
        private const string COMPANION1_CONFIG_NAME = "Companion1";
        private const string COMPANION2_CONFIG_NAME = "Companion2";

        public List<Hero> Heroes { get; private set; }

        protected override void Init()
        {
            Heroes = new List<Hero>
            {
                new(string.Format(BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH, HERO_CONFIG_NAME, HERO_CONFIG_NAME)),
                new(string.Format(BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH, COMPANION1_CONFIG_NAME, COMPANION1_CONFIG_NAME)),
                new(string.Format(BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH, COMPANION2_CONFIG_NAME, COMPANION2_CONFIG_NAME))
            };
        }

        public void FullHealTeam() => Heroes.ForEach(hero => hero.FullHeal());
    }
}