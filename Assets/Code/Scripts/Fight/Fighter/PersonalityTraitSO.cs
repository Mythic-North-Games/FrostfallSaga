using UnityEngine;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "PersonalityTrait", menuName = "ScriptableObjects/Fight/PersonailtyTrait", order = 0)]
    public class PersonalityTraitSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        [field: SerializeField, Tooltip("Used to instanciate the associated FighterBehaviourTree.")]
        public EFighterBehaviourTreeID FighterBehaviourTreeID { get; private set; }
    }
}