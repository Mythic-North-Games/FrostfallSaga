using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.DataStructures.BehaviourTree;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks
{
    /// <summary>
    ///     Check if the possessed fighter can move. Enough move points and no obstacles.
    /// </summary>
    public class CanMoveCheck : FBTNode
    {
        public CanMoveCheck(Fighter possessedFighter, FightHexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams) :
            base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        public override NodeState Evaluate()
        {
            return _possessedFighter.CanMove(_fightGrid) ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}