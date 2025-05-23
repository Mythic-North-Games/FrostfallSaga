using System;
using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    [CreateAssetMenu(fileName = "FighterConfiguration", menuName = "ScriptableObjects/Entities/FighterConfiguration",
        order = 0)]
    public class FighterConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public GameObject FighterPrefab { get; private set; }
        [field: SerializeField] public FighterClassSO FighterClass { get; private set; }
        [field: SerializeField] public APersonalityTrait PersonalityTrait { get; private set; }
        [field: SerializeField] public List<AActiveAbility> UnlockedActiveAbilities { get; private set; }
        [field: SerializeField] public int ActiveAbilitiesCapacity { get; private set; }
        [field: SerializeField] public List<APassiveAbility> UnlockedPassiveAbilities { get; private set; }
        [field: SerializeField] public int PassiveAbilitiesCapacity { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Item -> (Apparition chance at fight start, Max apparition count)")]
        private SElementToValue<ItemSO, (float apparitionChance, int maxApparitionCount)>[] _availableItems;

        public Dictionary<ItemSO, (float apparitionChance, int maxApparitionCount)> AvailableItems
        {
            get => SElementToValue<ItemSO, (float, int)>.GetDictionaryFromArray(_availableItems);
            private set => _availableItems = SElementToValue<ItemSO, (float, int)>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] public int MinStycasLoot { get; private set; }
        [field: SerializeField] public int MaxStycasLoot { get; private set; }

        public FighterConfigurationSO()
        {
            FighterPrefab = null;
            FighterClass = null;
            PersonalityTrait = null;
            UnlockedActiveAbilities = new();
            ActiveAbilitiesCapacity = 0;
            UnlockedPassiveAbilities = new();
            PassiveAbilitiesCapacity = 0;
            MaxHealth = 0;
            MaxActionPoints = 0;
            MaxMovePoints = 0;
            Strength = 0;
            Dexterity = 0;
            Tenacity = 0;
            PhysicalResistance = 0;
            MagicalResistances = new Dictionary<EMagicalElement, int>
            {
                { EMagicalElement.FIRE, 0 },
                { EMagicalElement.ICE, 0 },
                { EMagicalElement.LIGHTNING, 0 },
                { EMagicalElement.EARTH, 0 },
                { EMagicalElement.LIGHT, 0 },
                { EMagicalElement.DARKNESS, 0 }
            };
            MagicalStrengths = new Dictionary<EMagicalElement, int>
            {
                { EMagicalElement.FIRE, 0 },
                { EMagicalElement.ICE, 0 },
                { EMagicalElement.LIGHTNING, 0 },
                { EMagicalElement.EARTH, 0 },
                { EMagicalElement.LIGHT, 0 },
                { EMagicalElement.DARKNESS, 0 }
            };
            DodgeChance = 0;
            MasterstrokeChance = 0;
            Initiative = 0;
            HealSelfAnimationName = "";
            ReceiveDamageAnimationName = "";
            ReduceStatAnimationName = "";
            IncreaseStatAnimationName = "";
        }

        public virtual FighterStats ExtractFighterStats()
        {
            return new FighterStats
            {
                maxHealth = MaxHealth,
                health = MaxHealth,
                maxActionPoints = MaxActionPoints,
                actionPoints = MaxActionPoints,
                maxMovePoints = MaxMovePoints,
                movePoints = MaxMovePoints,
                strength = Strength,
                dexterity = Dexterity,
                tenacity = Tenacity,
                physicalResistance = PhysicalResistance,
                magicalResistances = MagicalResistances,
                magicalStrengths = MagicalStrengths,
                dodgeChance = DodgeChance,
                masterstrokeChance = MasterstrokeChance,
                initiative = Initiative
            };
        }

        #region Base stats #region Base stats

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int MaxHealth { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int MaxActionPoints { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int MaxMovePoints { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int Strength { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int Dexterity { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int Tenacity { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int PhysicalResistance { get; private set; }

        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _magicalResistances;
        public Dictionary<EMagicalElement, int> MagicalResistances
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_magicalResistances);
            private set => _magicalResistances = SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _magicalStrengths;
        public Dictionary<EMagicalElement, int> MagicalStrengths
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_magicalStrengths);
            private set => _magicalStrengths = SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField]
        [field: Range(0, 1)]
        public float DodgeChance { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        public float MasterstrokeChance { get; private set; }

        [field: SerializeField]
        [field: Range(0, 9999)]
        public int Initiative { get; private set; }

        #endregion

        #region Animations

        [field: SerializeField] public string HealSelfAnimationName { get; private set; }
        [field: SerializeField] public string ReceiveDamageAnimationName { get; private set; }
        [field: SerializeField] public string ReduceStatAnimationName { get; private set; }
        [field: SerializeField] public string IncreaseStatAnimationName { get; private set; }
        [field: SerializeField] public string ConsumableUseAnimationName { get; private set; }

        #endregion
    }
}