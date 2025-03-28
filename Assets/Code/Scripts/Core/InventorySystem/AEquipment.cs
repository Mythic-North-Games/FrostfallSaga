using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    public abstract class AEquipment : ItemSO
    {
        public abstract Dictionary<Sprite, string> GetStatsUIData();
        public abstract Dictionary<Sprite, string> GetMagicalStatsUIData();
        public abstract List<string> GetSpecialEffectsUIData();
    }
}