using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.Rewards;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core.HeroTeam
{
    public class HeroTeam : MonoBehaviourPersistingSingleton<HeroTeam>
    {
        private const string HERO_TEAM_CONFIGURATION_RESOURCE_PATH = "ScriptableObjects/Entities/HeroTeamConfiguration";
        private HeroTeamConfigurationSO _heroTeamConfiguration;

        /// <summary>
        /// Automatically initialize the singleton on scene load.
        /// </summary>
        static HeroTeam()
        {
            AutoInitializeOnSceneLoad = true;
        }

        public List<Hero> Heroes { get; private set; }
        public int Stycas => _heroTeamConfiguration.Stycas;

        protected override void Init()
        {
            _heroTeamConfiguration = Resources.Load<HeroTeamConfigurationSO>(HERO_TEAM_CONFIGURATION_RESOURCE_PATH);
            if (_heroTeamConfiguration == null)
            {
                Debug.LogError("HeroTeamConfigurationSO not found at path: " + HERO_TEAM_CONFIGURATION_RESOURCE_PATH);
                return;
            }

            Heroes = new List<Hero>
            {
                new(_heroTeamConfiguration.HeroEntityConfiguration),
                new(_heroTeamConfiguration.Companion2EntityConfiguration)
            };
        }

        /// <summary>
        /// Get the entity configurations of the alive heroes.
        /// </summary>
        /// <returns>The entity configurations of the alive heroes.</returns>
        public EntityConfigurationSO[] GetAliveHeroesEntityConfig()
        {
            return Heroes
                .Where(hero => !hero.IsDead())
                .ToList()
                .ConvertAll(hero => hero.EntityConfiguration)
                .ToArray();
        }

        /// <summary>
        /// Fully heal each hero of the team.
        /// </summary>
        public void FullHealTeam() => Heroes.ForEach(hero => hero.FullHeal());

        /// <summary>
        /// Makes the team collect the given reward.
        /// </summary>
        /// <param name="rewardToCollect">The reward to collect.</param>
        public void CollectReward(Reward rewardToCollect)
        {
            AddStycas(rewardToCollect.StycasEarned);
            DistributeItems(rewardToCollect.ItemsEarned);
        }

        /// <summary>
        /// Add stycas to the team.
        /// </summary>
        /// <param name="amount">The amount of stycas to add.</param>
        public void AddStycas(int amount) => _heroTeamConfiguration.Stycas += amount;

        /// <summary>
        /// Withdraw stycas from the team. Clamped to 0.
        /// </summary>
        /// <param name="amount">The amount of stycas to withdraw. Clamped to 0.</param>
        public void WithdrawStycas(int amount) =>
            _heroTeamConfiguration.Stycas = Math.Clamp(Stycas - amount, 0, int.MaxValue);

        /// <summary>
        /// Distribute the items to the heroes of the team.
        /// </summary>
        /// <param name="items">The items to distribute</param>
        public void DistributeItems(Dictionary<ItemSO, int> items)
        {
            foreach (KeyValuePair<ItemSO, int> lootedItem in items)
            {
                ItemSO item = lootedItem.Key;
                int itemCount = lootedItem.Value;

                for (int i = 0; i < itemCount; i++)
                {
                    DistributeItem(item);
                }
            }
        }

        /// <summary>
        /// Distribute a single item to the first free inventory slot of the heroes.
        /// </summary>
        /// <param name="item">The item to distribute</param>
        /// <remarks>
        /// If no free inventory slot is found, the item is not added to any hero's inventory.
        /// </remarks>
        public void DistributeItem(ItemSO item)
        {
            Inventory freeInventory = GetFirstFreeInventoryForItem(Heroes, item);
            if (freeInventory == null)
            {
                Debug.Log("No free inventory slot found for looted item");
                return;
            }
            freeInventory.AddItemToBag(item);
        }

        private static Inventory GetFirstFreeInventoryForItem(List<Hero> heroes, ItemSO item)
        {
            return heroes
                .Select(hero => hero.PersistedFighterConfiguration.Inventory)
                .FirstOrDefault(inventory => inventory.CanAddItemToBag(item));
        }
    }
}