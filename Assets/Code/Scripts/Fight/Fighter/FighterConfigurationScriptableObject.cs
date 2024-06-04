using FrostfallSaga.Fight.Targeters;
using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/Fight/Fighter", order = 0)]
    public class FighterConfigurationScriptableObject : ScriptableObject
    {
        [field: SerializeField] public ActiveAbilitiesToAnimation[] ActiveAbilities { get; private set; }
        [field: SerializeField] public TargeterScriptableObject DirectAttackTargeter { get; private set; }
        [field: SerializeField] public int DirectAttackDamageAmount { get; private set; }

        #region Base stats
        [field: SerializeField, Range(1, 9999)] public int MaxHealth { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int MaxActionPoints { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int MaxMovePoints { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int Strength { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int Dexterity { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int MagicalStrength { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int PhysicalResistance { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int MagicalResistance { get; private set; }
        [field: SerializeField, Range(1, 9999)] public int Initiative { get; private set; }
        #endregion
    }
}