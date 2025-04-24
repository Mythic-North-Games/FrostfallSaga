using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Trees;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    [CreateAssetMenu(fileName = "FighterClass", menuName = "ScriptableObjects/Fight/FighterClass", order = 0)]
    public class FighterClassSO : ScriptableObject
    {
        [field: SerializeField] public string ClassName { get; private set; }
        [field: SerializeField] public int ClassMaxHealth { get; private set; }
        [field: SerializeField] public int ClassMaxActionPoints { get; private set; }
        [field: SerializeField] public int ClassMaxMovePoints { get; private set; }
        [field: SerializeField] public int ClassStrength { get; private set; }
        [field: SerializeField] public int ClassDexterity { get; private set; }
        [field: SerializeField] public int ClassTenacity { get; private set; }
        [field: SerializeField] public int ClassPhysicalResistance { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] ClassMagicalResistances { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] ClassMagicalStrengths { get; private set; }
        [field: SerializeField] public float ClassDodgeChance { get; private set; }
        [field: SerializeField] public float ClassMasterstrokeChance { get; private set; }
        [field: SerializeField] public int ClassInitiative { get; private set; }
        [field: SerializeField] public ClassGodSO God { get; private set; }
        [field: SerializeField] public GraphNode<ABaseAbility> AbilitiesGraphModel { get; private set; }

        public void SetGraphRoot(GraphNode<ABaseAbility> root)
        {
            AbilitiesGraphModel = root;
        }
    }
}
