using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils.Trees.BehaviourTree;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees
{
    /// <summary>
    /// Base class for fighter behaviour trees.
    /// </summary>
    public abstract class FighterBehaviourTree : BTree
    {
        protected readonly Fighter _possessedFighter;
        protected readonly AHexGrid _fightGrid;
        protected readonly Dictionary<Fighter, bool> _fighterTeams;

        /// <summary>
        /// FighterBehaviourTrees constructor.
        /// </summary>
        /// <param name="possessedFighter">The fighter to control.</param>
        /// <param name="fightGrid">The fight grid.</param>
        /// <param name="fighterTeams">All the fighter and their corresponding teams.</param>
        /// <exception cref="MalformedFBTException">Thrown if tree not set up correctly.</exception>
        public FighterBehaviourTree(Fighter possessedFighter, AHexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            _possessedFighter = possessedFighter;
            _fightGrid = fightGrid;
            _fighterTeams = fighterTeams;
            _root = SetupTree();

            if (_root == null)
            {
                throw new MalformedFBTException("Tree does not contains a root node.");
            }
            if (_root.parent != null)
            {
                throw new MalformedFBTException("Root node has a parent.");
            }
            if (!ContainsEndTurnAction(_root))
            {
                throw new MalformedFBTException("Tree does not contains an EndTurnAction. WIll loop forever.");
            }
        }

        /// <summary>
        /// Returns true if the possessed fighter is doing an action. 
        /// Use this to stop running the tree until the action is done.
        /// </summary>
        /// <returns>True if the possessed fighter is doing an action, false otherwise.</returns>
        public bool IsActionRunning()
        {
            object actionRunning = _root.GetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY);
            return actionRunning != null && (bool)actionRunning;
        }

        /// <summary>
        /// Returns true if the turn has ended. Use this to know when to stop the tree execution.
        /// </summary>
        /// <returns>True if the turn has ended, false otherwise.</returns>
        public bool HasTurnEnded()
        {
            object turnEnded = _root.GetSharedData(EndTurnAction.TURN_ENDED_SHARED_DATA_KEY);
            return turnEnded != null && (bool)turnEnded;
        }

        private bool ContainsEndTurnAction(Node node)
        {
            if (node is EndTurnAction)
            {
                return true;
            }
            foreach (var child in node.GetChildren())
            {
                if (ContainsEndTurnAction(child))
                {
                    return true;
                }
            }
            return false;
        }
    }
}