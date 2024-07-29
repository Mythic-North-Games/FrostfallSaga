using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.KingdomToFight
{
    [CreateAssetMenu(fileName = "PersistedFighter", menuName = "ScriptableObjects/Fight/PersistedFighter", order = 0)]
    public class PersistedFighterConfigurationSO : FighterConfigurationSO
    {
        [field: SerializeField] public ActiveAbilityToAnimation[] EquipedActiveAbilities { get; private set; }
        [field: SerializeField] public int Health { get; private set; }

        public override FighterStats ExtractFighterStats()
        {
            FighterStats stats = base.ExtractFighterStats();
            stats.health = Health;
            return stats;
        }
    }
}