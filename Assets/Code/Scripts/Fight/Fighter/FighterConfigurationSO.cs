using UnityEngine;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Targeters;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fight/Fighter", order = 0)]
    public class FighterConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public ActiveAbilityToAnimation[] AvailableActiveAbilities { get; private set; }
        [field: SerializeField] public TargeterSO DirectAttackTargeter { get; private set; }
        [field: SerializeField] public AEffectSO[] DirectAttackEffects { get; private set; }
        [field: SerializeField] public int DirectAttackActionPointsCost { get; private set; }
        [field: SerializeField] public string DirectAttackAnimationStateName { get; private set; }
        [field: SerializeField] public string HealSelfAnimationStateName { get; private set; }
        [field: SerializeField] public string ReceiveDamageAnimationStateName { get; private set; }

        #region Base stats
        [field: SerializeField, Range(1, 9999)] public int MaxHealth { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int MaxActionPoints { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int MaxMovePoints { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int Strength { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int Dexterity { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int PhysicalResistance { get; private set; }
        [field: SerializeField]
        public MagicalElementToValue[] MagicalResistances
        {
            get; private set;
        }
        [field: SerializeField]
        public MagicalElementToValue[] MagicalStrengths
        {
            get; private set;
        }
        [field: SerializeField, Range(1, 9999)] public int Initiative { get; private set; }
        #endregion
    }
}