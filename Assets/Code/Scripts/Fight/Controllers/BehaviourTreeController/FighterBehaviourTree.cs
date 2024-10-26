using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Controllers.BehaviourTreeController.Actions;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController
{
    /// <summary>
    /// Base class for fighter behaviour trees.
    /// </summary>
    public abstract class FighterBehaviourTree : BTree
    {
        protected readonly Fighter _possessedFighter;
        protected readonly HexGrid _fightGrid;
        protected readonly Dictionary<Fighter, bool> _fighterTeams;

        /// <summary>
        /// FighterBehaviourTrees constructor.
        /// </summary>
        /// <param name="possessedFighter">The fighter to control.</param>
        /// <param name="fightGrid">The fight grid.</param>
        /// <param name="fighterTeams">All the fighter and their corresponding teams.</param>
        /// <exception cref="MalformedFBTException">Thrown if tree not set up correctly.</exception>
        public FighterBehaviourTree(Fighter possessedFighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
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
        /// Returns true if the turn has ended. Use this to know when to stop the tree execution.
        /// </summary>
        /// <returns>True if the turn has ended, false otherwise.</returns>
        public bool HasTurnEnded()
        {
            return (bool)_root.GetSharedData("TurnEnded");
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