using System;
using FrostfallSaga.InventorySystem;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    [CreateAssetMenu(fileName = "PersistedFighterConfiguration",
        menuName = "ScriptableObjects/Entities/PersistedFighterConfiguration", order = 0)]
    public class PersistedFighterConfigurationSO : FighterConfigurationSO
    {
        [field: SerializeField] public ABaseAbility[] EquipedActiveAbilities { get; private set; }
        [field: SerializeField] public ABaseAbility[] EquipedPassiveAbilities { get; private set; }
        [field: SerializeField] public Inventory Inventory { get; private set; }
        [field: SerializeField] public int Health { get; private set; }

        public PersistedFighterConfigurationSO()
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

        public void SetHealth(int amount)
        {
            if (amount < 0)
            {
                Health = 0;
            }
            else if (amount > MaxHealth)
            {
                Health = MaxHealth;
                Debug.LogWarning("Tried to set health above max health. Setting to max health.");
            }
            else
            {
                Health = amount;
            }
        }
    }
}