namespace FrostfallSaga.Utils.DataStructures.BehaviourTree
{
    /// <summary>
    ///     Base class for any behaviour tree.
    ///     This class is intended to be inherited from and the SetupTree method should be overridden to define the tree.
    ///     If not overriden by a child class, the `Execute` method will launch one evaluation of the tree.
    /// </summary>
    public abstract class BTree
    {
        protected Node _root;

        public BTree()
        {
            _root = SetupTree();
        }

        /// <summary>
        ///     Executes the tree once and returns if it succeeded or not.
        /// </summary>
        /// <returns>True if the tree succeeded, false otherwise.</returns>
        public virtual bool Execute()
        {
            if (_root == null)
                return false;
            return _root.Evaluate() == NodeState.SUCCESS;
        }

        /// <summary>
        ///     Override this method to define the tree structure.
        /// </summary>
        /// <returns>The root node of the entire tree.</returns>
        protected abstract Node SetupTree();
    }
}