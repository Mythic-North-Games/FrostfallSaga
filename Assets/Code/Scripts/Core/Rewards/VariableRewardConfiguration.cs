using System;
using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core.Rewards
{
    [Serializable]
    public class VariableRewardConfiguration
    {
        [field: SerializeField] public int MinStycas { get; private set; }
        [field: SerializeField] public int MaxStycas { get; private set; }

        [field: SerializeField]
        private SElementToValue<ItemSO, SElementToValue<float, int>>[] _possibleItems;
        public Dictionary<ItemSO, SElementToValue<float, int>> PossibleItems
        {
            get => SElementToValue<ItemSO, SElementToValue<float, int>>.GetDictionaryFromArray(_possibleItems);
            private set => _possibleItems = SElementToValue<ItemSO, SElementToValue<float, int>>.GetArrayFromDictionary(value);
        }

        /// <summary>
        /// Generate a random amount of stycas between MinStycas and MaxStycas
        /// </summary>
        /// <returns>A random amount of stycas between MinStycas and MaxStycas</returns>
        public int GenerateStycasReward() => Randomizer.GetRandomIntBetween(MinStycas, MaxStycas);

        /// <summary>
        /// Generate an array of items that will be rewarded depending on their configured apparition chance and count.
        /// </summary>
        /// <returns>The array of items that will be rewarded.</returns>
        public Dictionary<ItemSO, int> GenerateItemsReward()
        {
            Dictionary<ItemSO, int> rewardedItems = new();

            foreach (KeyValuePair<ItemSO, SElementToValue<float, int>> item in PossibleItems)
            {
                ItemSO possibleItem = item.Key;
                float includeInRewardChance = item.Value.element;
                int maxPossibleItemCount = item.Value.value;

                for (int i = 0; i < maxPossibleItemCount; i++)
                {
                    if (Randomizer.GetBooleanOnChance(includeInRewardChance))
                    {
                        // Check if the item is already in the list
                        if (rewardedItems.ContainsKey(possibleItem))
                        {
                            // If it is, increase the count
                            rewardedItems[possibleItem]++;
                        }
                        else
                        {
                            // If not, add it to the list with a count of 1
                            rewardedItems.Add(possibleItem, 1);
                        }
                    }
                }
            }

            return rewardedItems;
        }
    }
}