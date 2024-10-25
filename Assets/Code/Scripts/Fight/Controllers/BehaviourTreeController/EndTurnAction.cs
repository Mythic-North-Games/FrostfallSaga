using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController
{
    public class EndTurnAction : FBTNode
    {
        public EndTurnAction(Fighter possessedFighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        /// <summary>
        /// Set "TurnEnded" in shared context data to true to indicate that fighter finished his turn.
        /// </summary>
        /// <returns>Always SUCCESS</returns>
        public override NodeState Evaluate()
        {
            SetSharedData("TurnEnded", true);
            return NodeState.SUCCESS;
        }
    }
}