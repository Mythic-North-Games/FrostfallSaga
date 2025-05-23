using System;
using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core.Rewards
{
    [Serializable]
    public class Reward
    {
        public Reward()
        {
            StycasEarned = 0;
            _itemsEarned = null;
        }

        public Reward(int stycasEarned, Dictionary<ItemSO, int> itemsEarned)
        {
            StycasEarned = stycasEarned;
            ItemsEarned = itemsEarned;
        }

        public int StycasEarned;
        [SerializeField] private SElementToValue<ItemSO, int>[] _itemsEarned;
        public Dictionary<ItemSO, int> ItemsEarned
        {
            get => SElementToValue<ItemSO, int>.GetDictionaryFromArray(_itemsEarned);
            set => _itemsEarned = value == null ? null : SElementToValue<ItemSO, int>.GetArrayFromDictionary(value);
        }
    }
}