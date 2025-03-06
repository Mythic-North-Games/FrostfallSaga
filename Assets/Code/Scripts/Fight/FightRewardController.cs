using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight
{
    public class FightRewardController : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;

        public Action onTeamRewarded;

        private void GenerateReward(Fighter[] allies, Fighter[] enemies)
        {
            // If allies have lost, don't generate any rewards
            if (allies.All(ally => ally.IsDead()))
            {
                return;
            }

            // Generate inventory loot for each alive enemy
            List<ItemSO> allInventoriesLoot = new();
            enemies.ToList().ForEach(enemy => allInventoriesLoot.AddRange(LootInventory(enemy.Inventory)));

            // Generate gold loot
            int goldLoot = 0;
            enemies.ToList().ForEach(enemy => goldLoot += Randomizer.GetRandomIntBetween(
                enemy.MinStycasLoot, enemy.MaxStycasLoot
            ));

            // Add loot to hero team inventory
            HeroTeam heroTeam = HeroTeam.Instance;
            foreach (ItemSO lootedItem in allInventoriesLoot)
            {
                Inventory freeInventory = GetFirstFreeInventoryForItem(heroTeam.Heroes, lootedItem);
                if (freeInventory == null)
                {
                    Debug.Log("No free inventory slot found for looted item");
                    break;
                }
                freeInventory.AddItemToBag(lootedItem);
            }
            heroTeam.AddStycas(goldLoot);

            onTeamRewarded?.Invoke();
        }

        private ItemSO[] LootInventory(Inventory inventory)
        {
            List<ItemSO> loot = new();

            foreach (ItemSO inventoryItem in inventory.GetAllItems())
            {
                if (Randomizer.GetBooleanOnChance(inventoryItem.LootChance))
                {
                    loot.Add(inventoryItem);
                }
            }

            return loot.ToArray();
        }

        private Inventory GetFirstFreeInventoryForItem(List<Hero> heroes, ItemSO item)
        {
            return heroes
                .Select(hero => hero.PersistedFighterConfiguration.Inventory)
                .FirstOrDefault(inventory => inventory.CanAddItemToBag(item));
        }

        #region Setup
        private void Awake()
        {
            if (_fightManager == null)
            {
                Debug.LogError("FightManager is not assigned in FightRewardGenerator");
                return;
            }

            _fightManager.onFightEnded += GenerateReward;
        }
        #endregion
    }
}