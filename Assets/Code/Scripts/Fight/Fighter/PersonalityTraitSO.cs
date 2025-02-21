using UnityEngine;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "PersonalityTrait", menuName = "ScriptableObjects/Fight/PersonailtyTrait", order = 0)]
    public class PersonalityTraitSO : APersonalityTrait
    {
        [field: SerializeField, Tooltip("Used to instanciate the associated FighterBehaviourTree.")]
        public EFighterBehaviourTreeID FighterBehaviourTreeID { get; private set; }
    }
}