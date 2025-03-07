using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Items/Armor", order = 0)]
    public class ArmorSO : AArmor
    {
        [field: SerializeField] public int PhysicalResistance { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MagicalResistances { get; private set; }
        [field: SerializeField] public SElementToValue<EEntityRace, float>[] FightersStrengths { get; private set; }

        public override Dictionary<string, string> GetStatsUIData()
        {
            Dictionary<string, string> statsUIData = new()
            {
                { UIIcons.PHYSICAL_RESISTANCE.GetIconResourceName(), PhysicalResistance.ToString() }
            };
            return statsUIData;
        }

        public override Dictionary<EMagicalElement, string> GetMagicalStatsUIData()
        {
            Dictionary<EMagicalElement, string> magicalResistancesUIData = new();
            
            Dictionary<EMagicalElement, int> magicalResistances = SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(
                MagicalResistances
            );
            foreach (EMagicalElement magicalElement in magicalResistances.Keys)
            {
                magicalResistancesUIData.Add(magicalElement, magicalResistances[magicalElement].ToString());
            }

            return magicalResistancesUIData;
        }

        public override List<string> GetSpecialEffectsUIData()
        {
            List<string> specialEffectsUIData = new();
            
            Dictionary<EEntityRace, float> fightersStrengths = SElementToValue<EEntityRace, float>.GetDictionaryFromArray(
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