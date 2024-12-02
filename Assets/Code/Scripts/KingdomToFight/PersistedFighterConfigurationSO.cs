using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.GameItems;

namespace FrostfallSaga.KingdomToFight
{
    [CreateAssetMenu(fileName = "PersistedFighter", menuName = "ScriptableObjects/Fight/PersistedFighter", order = 0)]
    public class PersistedFighterConfigurationSO : FighterConfigurationSO
    {
        [field: SerializeField] public ActiveAbilitySO[] EquipedActiveAbilities { get; private set; }
        [field: SerializeField] public PassiveAbilitySO[] EquipedPassiveAbilities { get; private set; }
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public Inventory Inventory { get; private set; }

        public PersistedFighterConfigurationSO() : base()
        {
            EquipedActiveAbilities = Array.Empty<ActiveAbilitySO>();
            EquipedPassiveAbilities = Array.Empty<PassiveAbilitySO>();
            Health = 0;
        }

        public override FighterStats ExtractFighterStats()
        {
            FighterStats stats = base.ExtractFighterStats();
            stats.health = Health;
            return stats;
        }
    }
}