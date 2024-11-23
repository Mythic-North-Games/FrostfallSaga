using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.GameItems
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Items/Weapon", order = 0)]
    public class WeaponSO : ItemSO
    {
        [field: SerializeField] public Targeter AttackTargeter { get; private set; }
        [field: SerializeField] public int UseActionPointsCost { get; private set; }
        [field: SerializeField] public int MinPhysicalDamages { get; private set; }
        [field: SerializeField] public int MaxPhysicalDamages { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MinMagicalDamages { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MaxMagicalDamages { get; private set; }
        [field: SerializeField] public SElementToValue<EntityID, float>[] FightersStrenghts { get; private set; }
        [SerializeReference] public List<AEffect> SpecialEffects;

        public WeaponSO()
        {
            SlotTag = EItemSlotTag.WEAPON;
        }

        public AEffect[] GetWeaponEffects(EntityID targetEntityID, bool atMax = false)
        {
            List<AEffect> effects = new()
            {
                GetPhysicalDamagesEffect(targetEntityID, atMax)
            };
            effects.AddRange(GetMagicalDamagesEffect(targetEntityID));
            effects.AddRange(SpecialEffects);
            return effects.ToArray();
        }

        public int GetPotentialsDamagesOnTarget(Fighter holder, Fighter target)
        {
            return GetWeaponEffects(target.EntityID).ToList().Sum(
                effect => effect.GetPotentialEffectDamages(holder, target, true)
            );
        }

        #region Physical damages computation

        public PhysicalDamageEffect GetPhysicalDamagesEffect(EntityID targetEntityID, bool atMax = false)
        {
            int finalDamages = GetComputedPhysicalDamages(targetEntityID, atMax);
            return new PhysicalDamageEffect(finalDamages);
        }

        private int GetComputedPhysicalDamages(EntityID targetEntityID, bool atMax = false)
        {
            int finalDamages = GetRandomPhysicalDamagesInRange(atMax);
            Dictionary<EntityID, float> fightersStrength = SElementToValue<EntityID, float>.GetDictionaryFromArray(
                FightersStrenghts
            );

            if (fightersStrength.Keys.Contains(targetEntityID))
            {
                finalDamages = (int)(fightersStrength[targetEntityID] * finalDamages);
            }
            return finalDamages;
        }

        private int GetRandomPhysicalDamagesInRange(bool atMax = false)
        {
            if (atMax)
            {
                return MaxPhysicalDamages;
            }

            if (MaxPhysicalDamages == 0)
            {
                return MinPhysicalDamages;
            }
            return Randomizer.GetRandomIntBetween(MinPhysicalDamages, MaxPhysicalDamages);
        }

        #endregion

        #region Magical damages computation

        public MagicalDamageEffect[] GetMagicalDamagesEffect(EntityID targetEntityID, bool atMax = false)
        {
            Dictionary<EMagicalElement, int> finalDamages = GetComputedMagicalDamages(targetEntityID, atMax);
            List<MagicalDamageEffect> magicalDamageEffects = new();
            foreach (EMagicalElement magicalElement in finalDamages.Keys)
            {
                magicalDamageEffects.Add(new MagicalDamageEffect(finalDamages[magicalElement], magicalElement));
            }
            return magicalDamageEffects.ToArray();
        }

        private Dictionary<EMagicalElement, int> GetComputedMagicalDamages(EntityID targetEntityID, bool atMax = false)
        {
            Dictionary<EMagicalElement, int> finalDamages = GetRandomMagicalDamagesInRange(atMax);
            Dictionary<EntityID, float> fightersStrength = SElementToValue<EntityID, float>.GetDictionaryFromArray(
                FightersStrenghts
            );

            if (fightersStrength.Keys.Contains(targetEntityID))
            {
                foreach (EMagicalElement magicalElement in finalDamages.Keys)
                {
                    finalDamages[magicalElement] = (int)(fightersStrength[targetEntityID] * finalDamages[magicalElement]);
                }
            }
            return finalDamages;
        }

        private Dictionary<EMagicalElement, int> GetRandomMagicalDamagesInRange(bool atMax = false)
        {
            Dictionary<EMagicalElement, int> minMagicalDamages = SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(
                MinMagicalDamages
            );
            Dictionary<EMagicalElement, int> maxMagicalDamages = SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(
                MaxMagicalDamages
            );

            Dictionary<EMagicalElement, int> finalMagicalDamages = new();
            foreach (EMagicalElement magicalElement in minMagicalDamages.Keys)
            {
                if (atMax)
                {
                    finalMagicalDamages.Add(magicalElement, maxMagicalDamages[magicalElement]);
                    continue;
                }

                if (maxMagicalDamages[magicalElement] == 0)
                {
                    finalMagicalDamages.Add(magicalElement, minMagicalDamages[magicalElement]);
                    continue;
                }

                finalMagicalDamages.Add(
                    magicalElement,
                    Randomizer.GetRandomIntBetween(minMagicalDamages[magicalElement], maxMagicalDamages[magicalElement])
                );
            }
            return finalMagicalDamages;
        }

        #endregion
    }
}
