using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.InventorySystem;
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
        public int Stycas { get; private set; }

        protected override void Init()
        {
            Heroes = new List<Hero>
            {
                new(string.Format(BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH, HERO_CONFIG_NAME, HERO_CONFIG_NAME)),
                new(string.Format(BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH, COMPANION1_CONFIG_NAME, COMPANION1_CONFIG_NAME)),
                new(string.Format(BASE_ENTITY_CONFIGURATION_SO_CONFIG_PATH, COMPANION2_CONFIG_NAME, COMPANION2_CONFIG_NAME))
            };
            FullHealTeam(); // * For now, we fully heal the team on initialization.
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
        /// Add stycas to the team.
        /// </summary>
        /// <param name="amount">The amount of stycas to add.</param>
        public void AddStycas(int amount) => Stycas += amount;

        /// <summary>
        /// Withdraw stycas from the team. Clamped to 0.
        /// </summary>
        /// <param name="amount">The amount of stycas to withdraw. Clamped to 0.</param>
        public void WithdrawStycas(int amount) => Stycas = Math.Clamp(Stycas - amount, 0, int.MaxValue);

        /// <summary>
        /// Distribute the items to the heroes of the team.
        /// </summary>
        /// <param name="items">The items to distribute</param>
        public void DistributeItems(ItemSO[] items)
        {
            foreach (ItemSO lootedItem in items)
            {
                Inventory freeInventory = GetFirstFreeInventoryForItem(Heroes, lootedItem);
                if (freeInventory == null)
                {
                    Debug.Log("No free inventory slot found for looted item");
                    break;
                }
                freeInventory.AddItemToBag(lootedItem);
            }
        }

        private Inventory GetFirstFreeInventoryForItem(List<Hero> heroes, ItemSO item)
        {
            return heroes
                .Select(hero => hero.PersistedFighterConfiguration.Inventory)
                .FirstOrDefault(inventory => inventory.CanAddItemToBag(item));
        }

        /// <summary>
        /// Automatically initialize the singleton on scene load.
        /// </summary>
        static HeroTeam()
        {
            AutoInitializeOnSceneLoad = true;
        }
    }
}