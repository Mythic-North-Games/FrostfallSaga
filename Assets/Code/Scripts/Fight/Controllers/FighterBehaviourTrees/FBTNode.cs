using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;  
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees
{
    /// <summary>
    /// Base class for Fighter Behaviour Tree nodes.
    /// Contains a reference to the possessed fighter and the fight grid.
    /// </summary>
    public abstract class FBTNode : Node
    {
        public static readonly string ACTION_RUNNING_SHARED_DATA_KEY = "ActionRunning";

        protected Fighter _possessedFighter;
        protected HexGrid _fightGrid;
        protected Dictionary<Fighter, bool> _fighterTeams;

        public FBTNode(Fighter possessedFighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            _possessedFighter = possessedFighter;
            _fightGrid = fightGrid;
            _fighterTeams = fighterTeams;
        }
    }
}