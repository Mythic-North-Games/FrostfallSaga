using System;
using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core
{
    [Serializable]
    public class RewardConfiguration
    {
        [field: SerializeField] public int MinStycas { get; private set; }
        [field: SerializeField] public int MaxStycas { get; private set; }

        [field: SerializeField]
        public SElementToValue<ItemSO, SElementToValue<float, int>>[] PossibleItems { get; private set; }

        /// <summary>
        /// Generate a random amount of stycas between MinStycas and MaxStycas
        /// </summary>
        /// <returns>A random amount of stycas between MinStycas and MaxStycas</returns>
        public int GenerateStycasReward() => Randomizer.GetRandomIntBetween(MinStycas, MaxStycas);

        /// <summary>
        /// Generate an array of items that will be rewarded depending on their configured apparition chance and count.
        /// </summary>
        /// <returns>The array of items that will be rewarded.</returns>
        public ItemSO[] GenerateItemsReward()
        {
            List<ItemSO> rewardedItems = new();

            Dictionary<ItemSO, SElementToValue<float, int>> possibleItems =
                SElementToValue<ItemSO, SElementToValue<float, int>>.GetDictionaryFromArray(PossibleItems);
            foreach (KeyValuePair<ItemSO, SElementToValue<float, int>> item in possibleItems)
            {
                ItemSO possibleItem = item.Key;
                float includeInRewardChance = item.Value.element;
                int maxPossibleItemCount = item.Value.value;

                for (int i = 0; i < maxPossibleItemCount; i++)
                {
                    if (Randomizer.GetBooleanOnChance(includeInRewardChance))
                    {
                        rewardedItems.Add(possibleItem);
                    }
                }
            }

            return rewardedItems.ToArray();
        }
    }
}