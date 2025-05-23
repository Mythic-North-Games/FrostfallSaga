using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Utils.DataStructures.GraphNode;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    [CreateAssetMenu(fileName = "PersistedFighterConfiguration",
        menuName = "ScriptableObjects/Entities/PersistedFighterConfiguration", order = 0)]
    public class PersistedFighterConfigurationSO : FighterConfigurationSO
    {
        [field: SerializeField] public List<AActiveAbility> EquippedActiveAbilities { get; private set; }
        [field: SerializeField] public List<APassiveAbility> EquippedPassiveAbilities { get; private set; }
        [field: SerializeField] public int AbilityPoints { get; private set; }
        [field: SerializeField] public Inventory Inventory { get; private set; }
        [field: SerializeField] public int Health { get; private set; }

        public new int ActiveAbilitiesCapacity = 5;
        public new int PassiveAbilitiesCapacity = 0;

        public PersistedFighterConfigurationSO()
        {
            EquippedActiveAbilities = new();
            EquippedPassiveAbilities = new();
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

        public EAbilityState GetAbilityState(ABaseAbility ability)
        {
            // If already in unlocked abilities, return unlocked state
            if (UnlockedActiveAbilities.Contains(ability) || UnlockedPassiveAbilities.Contains(ability))
            {
                return EAbilityState.Unlocked;
            }

            // Otherwise, check if all parents are unlocked
            GraphNode<ABaseAbility> node = GraphNode<ABaseAbility>.FindInGraph(FighterClass.AbilitiesGraphRoot, ability);
            if (node == null)
            {
                Debug.LogError($"Ability {ability.Name} not found in graph.");
                return EAbilityState.Locked;
            }

            // If all parents are unlocked, return unlockable state
            if (node.Parents.Count == 0 || node.Parents.All(
                parent => UnlockedActiveAbilities.Contains(parent.Data) || UnlockedPassiveAbilities.Contains(parent.Data)
            ))
            {
                return EAbilityState.Unlockable;
            }

            // If not all parents are unlocked, return locked state
            return EAbilityState.Locked;
        }

        public void EquipAbility(ABaseAbility ability)
        {
            if (ability is AActiveAbility activeAbility)
            {
                if (EquippedActiveAbilities.Contains(activeAbility))
                {
                    Debug.Log($"Ability {activeAbility.Name} is already equipped.");
                    return;
                }

                // Replace the last equipped ability if the capacity is reached
                if (EquippedActiveAbilities.Count >= ActiveAbilitiesCapacity)
                {
                    EquippedActiveAbilities.RemoveAt(-1);
                }

                EquippedActiveAbilities.Add(activeAbility);
            }
            else if (ability is APassiveAbility passiveAbility)
            {
                if (EquippedPassiveAbilities.Contains(passiveAbility))
                {
                    Debug.Log($"Ability {passiveAbility.Name} is already equipped.");
                    return;
                }

                // Replace the last equipped ability if the capacity is reached
                if (EquippedPassiveAbilities.Count >= PassiveAbilitiesCapacity)
                {
                    EquippedPassiveAbilities.RemoveAt(-1);
                }

                EquippedPassiveAbilities.Add(passiveAbility);
            }
        }

        public bool UnequipAbility(ABaseAbility ability)
        {
            if (ability is AActiveAbility activeAbility)
            {
                return EquippedActiveAbilities.Remove(activeAbility);
            }
            else if (ability is APassiveAbility passiveAbility)
            {
                return EquippedPassiveAbilities.Remove(passiveAbility);
            }
            else
            {
                Debug.LogError($"Ability {ability.Name} is not an active or passive ability.");
                return false;
            }
        }

        public bool IsAbilityEquipped(ABaseAbility ability)
        {
            if (ability is AActiveAbility activeAbility)
            {
                return EquippedActiveAbilities.Contains(activeAbility);
            }
            else if (ability is APassiveAbility passiveAbility)
            {
                return EquippedPassiveAbilities.Contains(passiveAbility);
            }
            else
            {
                Debug.LogError($"Ability {ability.Name} is not an active or passive ability.");
                return false;
            }
        }

        public void UnlockAbility(ABaseAbility ability)
        {
            if (GetAbilityState(ability) != EAbilityState.Unlockable)
            {
                Debug.LogError($"Ability {ability.Name} is not unlockable.");
                return;
            }

            if (AbilityPoints < ability.UnlockPoints)
            {
                Debug.LogError($"Not enough ability points to unlock {ability.Name}. Required: {ability.UnlockPoints}, Available: {AbilityPoints}");
                return;
            }

            if (ability is AActiveAbility activeAbility) UnlockedActiveAbilities.Add(activeAbility);
            else if (ability is APassiveAbility passiveAbility) UnlockedPassiveAbilities.Add(passiveAbility);
            AbilityPoints -= ability.UnlockPoints;
        }
    }
}