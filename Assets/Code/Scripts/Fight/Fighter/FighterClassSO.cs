using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "FighterClass", menuName = "ScriptableObjects/Fight/FighterClass", order = 0)]
    public class FighterClassSO : ScriptableObject
    {
        [field: SerializeField] public int ClassMaxHealth { get; private set; }
        [field: SerializeField] public int ClassMaxActionPoints { get; private set; }
        [field: SerializeField] public int ClassMaxMovePoints { get; private set; }
        [field: SerializeField] public int ClassStrength { get; private set; }
        [field: SerializeField] public int ClassDexterity { get; private set; }
        [field: SerializeField] public float ClassTenacity { get; private set; }
        [field: SerializeField] public int ClassPhysicalResistance { get; private set; }
        [field: SerializeField] public MagicalElementToValue[] ClassMagicalResistances { get; private set; }
        [field: SerializeField] public MagicalElementToValue[] ClassMagicalStrengths { get; private set; }
        [field: SerializeField] public float ClassDodgeChance { get; private set; }
        [field: SerializeField] public float ClassMasterstrokeChance { get; private set; }
        [field: SerializeField] public int ClassInitiative { get; private set; }
        [field: SerializeField] public ClassGodSO God { get; private set; }
    }
}