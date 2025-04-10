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

        public static List<ItemSO> GenerateItemsReward(Fighter[] enemies)
        {
            List<ItemSO> allInventoriesLoot = new();
            enemies.ToList().ForEach(enemy => allInventoriesLoot.AddRange(LootInventory(enemy.Inventory)));
            return allInventoriesLoot;
        }

        private static ItemSO[] LootInventory(Inventory inventory)
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
    }
}