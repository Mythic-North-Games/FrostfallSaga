using System.Collections.Generic;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Core.InventorySystem
{
    public abstract class AEquipment : ItemSO
    {
        public abstract Dictionary<string, string> GetStatsUIData();
        public abstract Dictionary<EMagicalElement, string> GetMagicalStatsUIData();
        public abstract List<string> GetSpecialEffectsUIData();
    }
}