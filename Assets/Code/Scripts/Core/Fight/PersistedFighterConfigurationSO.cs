using System;
using UnityEngine;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.InventorySystem;

namespace FrostfallSaga.Core.Fight
{
    [CreateAssetMenu(fileName = "PersistedFighter", menuName = "ScriptableObjects/Fight/PersistedFighter", order = 0)]
    public class PersistedFighterConfigurationSO : FighterConfigurationSO
    {
        [field: SerializeField] public ABaseAbility[] EquipedActiveAbilities { get; private set; }
        [field: SerializeField] public ABaseAbility[] EquipedPassiveAbilities { get; private set; }
        [field: SerializeField] public Inventory Inventory { get; private set; }
        [field: SerializeField] public int Health { get; private set; }

        public PersistedFighterConfigurationSO() : base()
        {
            EquipedActiveAbilities = Array.Empty<ABaseAbility>();
            EquipedPassiveAbilities = Array.Empty<ABaseAbility>();
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