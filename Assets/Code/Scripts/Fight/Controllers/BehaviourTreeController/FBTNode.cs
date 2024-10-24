using FrostfallSaga.BehaviourTree;  
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController
{
    /// <summary>
    /// Base class for Fighter Behaviour Tree nodes.
    /// Contains a reference to the possessed fighter and the fight grid.
    /// </summary>
    public abstract class FBTNode : Node
    {
        protected Fighter _possessedFighter;
        protected HexGrid _fightGrid;

        public FBTNode(Fighter possessedFighter, HexGrid fightGrid)
        {
            _possessedFighter = possessedFighter;
            _fightGrid = fightGrid;
        }
    }
}