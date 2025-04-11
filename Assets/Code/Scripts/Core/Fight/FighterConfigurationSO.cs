using System;
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
        [field: SerializeField] public ABaseAbility[] AvailableActiveAbilities { get; private set; }
        [field: SerializeField] public int ActiveAbilitiesCapacity { get; private set; }
        [field: SerializeField] public ABaseAbility[] AvailablePassiveAbilities { get; private set; }
        [field: SerializeField] public int PassiveAbilitiesCapacity { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Item -> (Apparition chance at fight start, Max apparition count)")]
        public SElementToValue<ItemSO, SElementToValue<float, int>>[] AvailableItems { get; private set; }

        [field: SerializeField] public int MinStycasLoot { get; private set; }
        [field: SerializeField] public int MaxStycasLoot { get; private set; }

        public FighterConfigurationSO()
        {
            FighterPrefab = null;
            FighterClass = null;
            PersonalityTrait = null;
            AvailableActiveAbilities = Array.Empty<ABaseAbility>();
            ActiveAbilitiesCapacity = 0;
            AvailablePassiveAbilities = Array.Empty<ABaseAbility>();
            PassiveAbilitiesCapacity = 0;
            MaxHealth = 0;
            MaxActionPoints = 0;
            MaxMovePoints = 0;
            Strength = 0;
            Dexterity = 0;
            Tenacity = 0;
            PhysicalResistance = 0;
            MagicalResistances = new SElementToValue<EMagicalElement, int>[]
            {
                new(EMagicalElement.FIRE, 0),
                new(EMagicalElement.ICE, 0),
                new(EMagicalElement.LIGHTNING, 0),
                new(EMagicalElement.EARTH, 0),
                new(EMagicalElement.LIGHT, 0),
                new(EMagicalElement.DARKNESS, 0)
            };
            MagicalResistances = new SElementToValue<EMagicalElement, int>[]
            {
                new(EMagicalElement.FIRE, 0),
                new(EMagicalElement.ICE, 0),
                new(EMagicalElement.LIGHTNING, 0),
                new(EMagicalElement.EARTH, 0),
                new(EMagicalElement.LIGHT, 0),
                new(EMagicalElement.DARKNESS, 0)
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
                magicalResistances = SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(MagicalResistances),
                magicalStrengths = SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(MagicalStrengths),
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

        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MagicalResistances { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MagicalStrengths { get; private set; }

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