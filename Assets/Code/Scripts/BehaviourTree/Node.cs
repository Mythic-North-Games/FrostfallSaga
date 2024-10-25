using System.Collections.Generic;

namespace FrostfallSaga.BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    /// <summary>
    /// Represents a single node in the behaviour tree.
    /// The evaluate method is where the behaviour logic is implemented. It must be overridden by the inheriting classes.
    /// </summary>
    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new();

        // Can contain context data for the entire tree. You can get and set data from any node in the tree.
        private readonly Dictionary<string, object> _sharedDataContext;

        public Node()
        {
            parent = null;
            _sharedDataContext = new Dictionary<string, object>();
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
                Attach(child);
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        /// <summary>
        /// Sets data in the context of the tree. Will be available to all nodes in the tree.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <param name="value">The value of the data.</param>
        public void SetSharedData(string key, object value)
        {
            if (parent == null)
            {
                _sharedDataContext[key] = value;
            }
            else
            {
                parent.SetSharedData(key, value);
            }
        }

        /// <summary>
        /// Gets data from the context of the tree. Available to all nodes in the tree.
        /// </summary>
        /// <param name="key">The key of the data to get.</param>
        public object GetSharedData(string key)
        {
            if (parent == null)
            {
                return _sharedDataContext.ContainsKey(key) ? _sharedDataContext[key] : null;
            }
            else
            {
                return parent.GetSharedData(key);
            }
        }

        /// <summary>
        /// Clears data from the context of the tree. The data will no longer be available to any node in the tree.
        /// </summary>
        /// <param name="key">The key of the data to clear.</param>
        /// <returns>True if the data has been successfully cleaned, false otherwise.</returns>
        public bool ClearSharedData(string key)
        {
            if (parent == null)
            {
                return _sharedDataContext.Remove(key);
            }
            else
            {
                return parent.ClearSharedData(key);
            }
        }
    }
}
