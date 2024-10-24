using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController
{
    public class CanMoveCheck : FBTNode
    {
        public CanMoveCheck(Fighter possessedFighter, HexGrid fightGrid) : base(possessedFighter, fightGrid)
        {
        }

        public override NodeState Evaluate()
        {
            return _possessedFighter.CanMove(_fightGrid) ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}