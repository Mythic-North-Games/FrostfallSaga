using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController
{
    public class EndTurnAction : FBTNode
    {
        public EndTurnAction(Fighter possessedFighter, HexGrid fightGrid) : base(possessedFighter, fightGrid)
        {
        }

        /// <summary>
        /// Set "TurnEnded" in shared context data to true to indicate that fighter finished his turn.
        /// </summary>
        /// <returns>Always SUCCESS</returns>
        public override NodeState Evaluate()
        {
            SetData("TurnEnded", true);
            return NodeState.SUCCESS;
        }
    }
}