using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController.Checks
{
    public class CanMoveCheck : FBTNode
    {
        public CanMoveCheck(Fighter possessedFighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        public override NodeState Evaluate()
        {
            return _possessedFighter.CanMove(_fightGrid) ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}