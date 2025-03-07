using System.Collections.Generic;
using FrostfallSaga.Utils.Trees.BehaviourTree;
using FrostfallSaga.Fight.Fighters;


namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks
{
    /// <summary>
    /// Check if the possessed fighter can move. Enough move points and no obstacles.
    /// </summary>
    public class CanMoveCheck : FBTNode
    {
        public CanMoveCheck(Fighter possessedFighter, FightHexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        public override NodeState Evaluate()
        {
            return _possessedFighter.CanMove(_fightGrid) ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}