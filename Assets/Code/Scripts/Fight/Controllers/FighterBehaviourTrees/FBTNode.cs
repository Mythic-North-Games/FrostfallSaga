using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.Trees.BehaviourTree;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees
{
    /// <summary>
    ///     Base class for Fighter Behaviour Tree nodes.
    ///     Contains a reference to the possessed fighter and the fight grid.
    /// </summary>
    public abstract class FBTNode : Node
    {
        public static readonly string ACTION_RUNNING_SHARED_DATA_KEY = "ActionRunning";
        public static readonly string TARGET_SHARED_DATA_KEY = "Target";
        protected Dictionary<Fighter, bool> _fighterTeams;
        protected FightHexGrid _fightGrid;

        protected Fighter _possessedFighter;

        protected FBTNode(Fighter possessedFighter, FightHexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            _possessedFighter = possessedFighter;
            _fightGrid = fightGrid;
            _fighterTeams = fighterTeams;
        }
    }
}