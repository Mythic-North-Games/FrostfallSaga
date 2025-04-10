using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.UI;
using UnityEngine;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Items/Armor", order = 0)]
    public class ArmorSO : AArmor
    {
        [field: SerializeField] public int PhysicalResistance { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MagicalResistances { get; private set; }
        [field: SerializeField] public SElementToValue<EEntityRace, float>[] FightersStrengths { get; private set; }

        public override Dictionary<Sprite, string> GetStatsUIData()
        {
            Dictionary<Sprite, string> statsUIData = new()
            {
                {
                    UIIconsProvider.Instance.GetIcon(UIIcons.PHYSICAL_RESISTANCE.GetIconResourceName()),
                    PhysicalResistance.ToString()
                }
            };
            return statsUIData;
        }

        public override Dictionary<Sprite, string> GetMagicalStatsUIData()
        {
            Dictionary<Sprite, string> magicalResistancesUIData = new();

            Dictionary<EMagicalElement, int> magicalResistances =
                SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(
                    MagicalResistances
                );
            foreach (EMagicalElement magicalElement in magicalResistances.Keys)
            {
                magicalResistancesUIData.Add(
                    UIIconsProvider.Instance.GetIcon(magicalElement.GetIconResourceName()),
                    magicalResistances[magicalElement].ToString()
                );
            }

            return magicalResistancesUIData;
        }

        public override List<string> GetSpecialEffectsUIData()
        {
            List<string> specialEffectsUIData = new();

            Dictionary<EEntityRace, float> fightersStrengths =
                SElementToValue<EEntityRace, float>.GetDictionaryFromArray(
                    FightersStrengths
                );
            foreach (EEntityRace entityRace in fightersStrengths.Keys)
            {
                string sign = fightersStrengths[entityRace] > 0 ? "+" : "-";
                int asPercentage = (int)((fightersStrengths[entityRace] - 1) * 100);
                specialEffectsUIData.Add($"{sign}{asPercentage}% resistance against {entityRace.ToUIString()}");
            }

            return specialEffectsUIData;
        }
    }
}