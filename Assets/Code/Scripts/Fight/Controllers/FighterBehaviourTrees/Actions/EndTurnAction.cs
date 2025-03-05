using System.Collections.Generic;
using FrostfallSaga.Utils.Trees.BehaviourTree;
using FrostfallSaga.Fight.Fighters;


namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions
{
    /// <summary>
    /// Set "TurnEnded" in shared context data to true to indicate that fighter finished his turn.
    /// </summary>
    public class EndTurnAction : FBTNode
    {
        public static string TURN_ENDED_SHARED_DATA_KEY = "TurnEnded";

        public EndTurnAction(
            Fighter possessedFighter,
            FightHexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        /// <summary>
        /// Set "TurnEnded" in shared context data to true to indicate that fighter finished his turn.
        /// </summary>
        /// <returns>Always SUCCESS</returns>
        public override NodeState Evaluate()
        {
            SetSharedData(TURN_ENDED_SHARED_DATA_KEY, true);
            return NodeState.SUCCESS;
        }
    }
}