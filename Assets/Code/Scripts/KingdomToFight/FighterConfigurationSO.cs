using UnityEngine;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;


namespace FrostfallSaga.KingdomToFight
{
    [CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fight/Fighter", order = 0)]
    public class FighterConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public FighterClassSO FighterClass { get; private set; }
        [field: SerializeField] public PersonalityTraitSO PersonalityTrait { get; private set; }
        [field: SerializeField] public ActiveAbilitySO[] AvailableActiveAbilities { get; private set; }
        [field: SerializeField] public int ActiveAbilitiesCapacity = 5;
        [field: SerializeField] public PassiveAbilitySO[] AvailablePassiveAbilities { get; private set; }
        [field: SerializeField] public int PassiveAbilitiesCapacity = 5;
        [field: SerializeField] public Targeter DirectAttackTargeter { get; private set; }
        [SerializeReference] public AEffect[] DirectAttackEffects = { };
        [field: SerializeField] public int DirectAttackActionPointsCost { get; private set; }

        #region Base stats
        [field: SerializeField, Range(0, 9999)] public int MaxHealth { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int MaxActionPoints { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int MaxMovePoints { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int Strength { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int Dexterity { get; private set; }
        [field: SerializeField, Range(0, 9999)] public float Tenacity { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int PhysicalResistance { get; private set; }
        [field: SerializeField] public MagicalElementToValue[] MagicalResistances { get; private set; }
        [field: SerializeField] public MagicalElementToValue[] MagicalStrengths { get; private set; }
        [field: SerializeField, Range(0, 9999)] public float DodgeChance { get; private set; }
        [field: SerializeField, Range(0, 9999)] public float MasterstrokeChance { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int Initiative { get; private set; }
        #endregion

        #region Animations
        [field: SerializeField] public AAbilityAnimationSO DirectAttackAnimation { get; private set; }
        [field: SerializeField] public string HealSelfAnimationName { get; private set; }
        [field: SerializeField] public string ReceiveDamageAnimationName { get; private set; }
        [field: SerializeField] public string ReduceStatAnimationName { get; private set; }
        [field: SerializeField] public string IncreaseStatAnimationName { get; private set; }
        #endregion

        public virtual FighterStats ExtractFighterStats()
        {
            return new()
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
                magicalResistances = MagicalElementToValue.GetDictionaryFromArray(MagicalResistances),
                magicalStrengths = MagicalElementToValue.GetDictionaryFromArray(MagicalStrengths),
                dodgeChance = DodgeChance,
                masterstrokeChance = MasterstrokeChance,
                initiative = Initiative
            };
        }
    }
}