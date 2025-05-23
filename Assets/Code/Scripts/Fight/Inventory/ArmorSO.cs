using System.Collections.Generic;
using System.Linq;
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
        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _magicalResistances;
        public Dictionary<EMagicalElement, int> MagicalResistances
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_magicalResistances);
            private set => _magicalResistances = SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] private SElementToValue<EEntityRace, float>[] _fightersStrengths;
        public Dictionary<EEntityRace, float> FightersStrengths
        {
            get => SElementToValue<EEntityRace, float>.GetDictionaryFromArray(_fightersStrengths);
            private set => _fightersStrengths = SElementToValue<EEntityRace, float>.GetArrayFromDictionary(value);
        }

        public override Dictionary<Sprite, string> GetStatsUIData()
        {
            Dictionary<Sprite, string> statsUIData = new()
            {
                {
                    UIIconsProvider.Instance.GetIcon(UIIcons.PHYSICAL_RESISTANCE.GetIconResourceName()),
                    PhysicalResistance.ToString()
                }
            };
            return statsUIData.Concat(
                GetMagicalStatsUIData()
            ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private Dictionary<Sprite, string> GetMagicalStatsUIData()
        {
            Dictionary<Sprite, string> magicalResistancesUIData = new();

            foreach (EMagicalElement magicalElement in MagicalResistances.Keys)
            {
                magicalResistancesUIData.Add(
                    UIIconsProvider.Instance.GetIcon(magicalElement.GetIconResourceName()),
                    MagicalResistances[magicalElement].ToString()
                );
            }

            return magicalResistancesUIData;
        }

        public override List<string> GetPrimaryEffectsUIData()
        {
            List<string> specialEffectsUIData = new();

            foreach (EEntityRace entityRace in FightersStrengths.Keys)
            {
                string sign = FightersStrengths[entityRace] > 0 ? "+" : "-";
                int asPercentage = (int)((FightersStrengths[entityRace] - 1) * 100);
                specialEffectsUIData.Add($"{sign}{asPercentage}% resistance against {entityRace.ToUIString()}");
            }

            return specialEffectsUIData;
        }
    }
}