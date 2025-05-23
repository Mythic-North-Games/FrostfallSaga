using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight
{
    public static class FightRewardGenerator
    {
        public static int GenerateStycasReward(Fighter[] enemies)
        {
            int stycasLoot = 0;
            enemies.ToList().ForEach(enemy => stycasLoot += Randomizer.GetRandomIntBetween(
                enemy.MinStycasLoot, enemy.MaxStycasLoot
            ));
            return stycasLoot;
        }

        public static Dictionary<ItemSO, int> GenerateItemsReward(Fighter[] enemies)
        {
            Dictionary<ItemSO, int> allInventoriesLoot = new();
            foreach (var enemy in enemies)
            {
                foreach (var item in LootInventory(enemy.Inventory))
                {
                    allInventoriesLoot[item.Key] = allInventoriesLoot.GetValueOrDefault(item.Key) + item.Value;
                }
            }
            return allInventoriesLoot;
        }

        private static Dictionary<ItemSO, int> LootInventory(Inventory inventory)
        {
            Dictionary<ItemSO, int> itemsToLoot = new();
            foreach (var inventoryItem in inventory.GetAllItems())
            {
                if (Randomizer.GetBooleanOnChance(inventoryItem.LootChance))
                    itemsToLoot[inventoryItem] = itemsToLoot.GetValueOrDefault(inventoryItem) + 1;
            }
            return itemsToLoot;
        }
    }
}