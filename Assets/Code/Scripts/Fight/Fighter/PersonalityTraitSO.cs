using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees;
using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "PersonalityTrait", menuName = "ScriptableObjects/Fight/PersonailtyTrait", order = 0)]
    public class PersonalityTraitSO : APersonalityTrait
    {
        [field: SerializeField]
        [field: Tooltip("Used to instanciate the associated FighterBehaviourTree.")]
        public EFighterBehaviourTreeID FighterBehaviourTreeID { get; private set; }
    }
}