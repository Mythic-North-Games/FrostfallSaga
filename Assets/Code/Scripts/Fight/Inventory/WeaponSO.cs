using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;
using FrostfallSaga.Utils;
using UnityEngine;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Audio;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Items/Weapon", order = 0)]
    public class WeaponSO : AWeapon
    {
        [field: SerializeField] public Targeter AttackTargeter { get; private set; }
        [field: SerializeField] public int UseActionPointsCost { get; private set; }
        [field: SerializeField] public int MinPhysicalDamages { get; private set; }
        [field: SerializeField] public int MaxPhysicalDamages { get; private set; }

        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _minMagicalDamages;
        public Dictionary<EMagicalElement, int> MinMagicalDamages
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_minMagicalDamages);
            private set => _minMagicalDamages = value == null ? null : SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _maxMagicalDamages;
        public Dictionary<EMagicalElement, int> MaxMagicalDamages
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_maxMagicalDamages);
            private set => _maxMagicalDamages = value == null ? null : SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] private SElementToValue<EEntityRace, float>[] _fightersStrengths;
        public Dictionary<EEntityRace, float> FightersStrengths
        {
            get => SElementToValue<EEntityRace, float>.GetDictionaryFromArray(_fightersStrengths);
            private set => _fightersStrengths = value == null ? null : SElementToValue<EEntityRace, float>.GetArrayFromDictionary(value);
        }

        [SerializeReference] public List<AEffect> specialEffects;
        [field: SerializeField] public AAbilityAnimationSO AttackAnimation { get; private set; }

        [field: SerializeField] public AudioClip WeaponUseSoundFX { get; private set; }

        public AEffect[] GetWeaponEffects(EEntityRace targetEntityID, bool atMax = false)
        {
            List<AEffect> effects = new()
            {
                GetPhysicalDamagesEffect(targetEntityID, atMax)
            };
            effects.AddRange(GetMagicalDamagesEffect(targetEntityID));
            effects.AddRange(specialEffects);
            return effects.ToArray();
        }

        public int GetPotentialsDamagesOnTarget(Fighter holder, Fighter target)
        {
            return GetWeaponEffects(target.Race).ToList().Sum(
                effect => effect.GetPotentialEffectDamages(holder, target, true)
            );
        }

        public void PlayUseSoundFXIfAny(Fighter initator)
        {
            if (WeaponUseSoundFX == null) return;

            Transform audioSourceTransform = initator.GetWeaponCollider() != null
                ? initator.GetWeaponCollider().transform
                : initator.transform;
            AudioManager.Instance.PlayFXSound(WeaponUseSoundFX, audioSourceTransform);
        }

        #region Physical damages computation

        public PhysicalDamageEffect GetPhysicalDamagesEffect(EEntityRace targetEntityID, bool atMax = false)
        {
            int finalDamages = GetComputedPhysicalDamages(targetEntityID, atMax);
            return new PhysicalDamageEffect(finalDamages);
        }

        private int GetComputedPhysicalDamages(EEntityRace targetEntityID, bool atMax = false)
        {
            int finalDamages = GetRandomPhysicalDamagesInRange(atMax);
            if (FightersStrengths == null || FightersStrengths.Count == 0) return finalDamages;

            if (FightersStrengths.Keys.Contains(targetEntityID))
                finalDamages = (int)(FightersStrengths[targetEntityID] * finalDamages);
            return finalDamages;
        }

        private int GetRandomPhysicalDamagesInRange(bool atMax = false)
        {
            if (atMax) return MaxPhysicalDamages;

            if (MaxPhysicalDamages == 0) return MinPhysicalDamages;
            return Randomizer.GetRandomIntBetween(MinPhysicalDamages, MaxPhysicalDamages);
        }

        #endregion

        #region Magical damages computation

        public MagicalDamageEffect[] GetMagicalDamagesEffect(EEntityRace targetEntityID, bool atMax = false)
        {
            Dictionary<EMagicalElement, int> finalDamages = GetComputedMagicalDamages(targetEntityID, atMax);
            List<MagicalDamageEffect> magicalDamageEffects = new();
            foreach (EMagicalElement magicalElement in finalDamages.Keys)
                magicalDamageEffects.Add(new MagicalDamageEffect(finalDamages[magicalElement], magicalElement));
            return magicalDamageEffects.ToArray();
        }

        private Dictionary<EMagicalElement, int> GetComputedMagicalDamages(EEntityRace targetEntityID,
            bool atMax = false)
        {
            Dictionary<EMagicalElement, int> finalDamages = GetRandomMagicalDamagesInRange(atMax);

            if (FightersStrengths.Keys.Contains(targetEntityID))
                foreach (EMagicalElement magicalElement in finalDamages.Keys)
                    finalDamages[magicalElement] =
                        (int)(FightersStrengths[targetEntityID] * finalDamages[magicalElement]);

            return finalDamages;
        }

        private Dictionary<EMagicalElement, int> GetRandomMagicalDamagesInRange(bool atMax = false)
        {
            Dictionary<EMagicalElement, int> finalMagicalDamages = new();
            foreach (EMagicalElement magicalElement in MinMagicalDamages.Keys)
            {
                if (atMax)
                {
                    finalMagicalDamages.Add(magicalElement, MaxMagicalDamages[magicalElement]);
                    continue;
                }

                if (MaxMagicalDamages[magicalElement] == 0)
                {
                    finalMagicalDamages.Add(magicalElement, MinMagicalDamages[magicalElement]);
                    continue;
                }

                finalMagicalDamages.Add(
                    magicalElement,
                    Randomizer.GetRandomIntBetween(MinMagicalDamages[magicalElement], MaxMagicalDamages[magicalElement])
                );
            }

            return finalMagicalDamages;
        }

        #endregion

        #region For the UI
        public override Dictionary<Sprite, string> GetStatsUIData()
        {
            UIIconsProvider iconsProvider = UIIconsProvider.Instance;

            string physicalDamagesString = MinPhysicalDamages != MaxPhysicalDamages ? $"{MinPhysicalDamages}-{MaxPhysicalDamages}" : MinPhysicalDamages.ToString();
            Dictionary<Sprite, string> statsUIData = new()
            {
                { iconsProvider.GetIcon(UIIcons.PHYSICAL_DAMAGE.GetIconResourceName()), physicalDamagesString },
                { iconsProvider.GetIcon(UIIcons.RANGE.GetIconResourceName()), AttackTargeter.OriginCellRange.ToString() },
                { iconsProvider.GetIcon(UIIcons.ACTION_POINTS_COST.GetIconResourceName()), UseActionPointsCost.ToString() }
            };
            return statsUIData.Concat(GetMagicalStatsUIData()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private Dictionary<Sprite, string> GetMagicalStatsUIData()
        {
            UIIconsProvider iconsProvider = UIIconsProvider.Instance;

            Dictionary<Sprite, string> magicalDamagesUIData = new();
            foreach (EMagicalElement magicalElement in MinMagicalDamages.Keys)
            {
                int minDamages = MinMagicalDamages[magicalElement];
                int maxDamages = MaxMagicalDamages[magicalElement];

                string magicalDanmagesString = minDamages != maxDamages ? $"{minDamages}-{maxDamages}" : minDamages.ToString();
                magicalDamagesUIData.Add(
                    iconsProvider.GetIcon(magicalElement.GetIconResourceName()),
                    magicalDanmagesString
                );
            }
            return magicalDamagesUIData;
        }

        public override List<string> GetPrimaryEffectsUIData()
        {
            List<string> specialEffectsUIData = new();
            specialEffects.ForEach(effect => specialEffectsUIData.Add(effect.GetUIEffectDescription()));

            foreach (EEntityRace entityRace in FightersStrengths.Keys)
            {
                string sign = FightersStrengths[entityRace] > 0 ? "+" : "-";
                int asPercentage = (int)((FightersStrengths[entityRace] - 1) * 100);
                specialEffectsUIData.Add($"{sign}{asPercentage}% damages against {entityRace.ToUIString()}");
            }

            return specialEffectsUIData;
        }
        #endregion
    }
}