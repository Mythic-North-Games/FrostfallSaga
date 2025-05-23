using FrostfallSaga.Core.Entities;
using UnityEngine;

namespace FrostfallSaga.Core.HeroTeam
{
    [CreateAssetMenu(fileName = "HeroTeamConfiguration",
        menuName = "ScriptableObjects/Entities/HeroTeamConfigurationSO", order = 0)]
    public class HeroTeamConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public EntityConfigurationSO HeroEntityConfiguration { get; private set; }
        [field: SerializeField] public EntityConfigurationSO Companion1EntityConfiguration { get; private set; }
        [field: SerializeField] public EntityConfigurationSO Companion2EntityConfiguration { get; private set; }
        public string HeroGroupName = "HeroGroup";
        public int Stycas;
    }
}