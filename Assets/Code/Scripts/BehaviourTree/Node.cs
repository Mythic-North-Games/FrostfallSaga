using System.Collections;
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
        protected List<Node> children = new List<Node>();

        // Can contain context data for the entire tree. You can get and set data from any node in the tree.
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }

        private void _Attach(Node node)
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
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        /// <summary>
        /// Gets data from the context of the tree. Available to all nodes in the tree.
        /// </summary>
        /// <param name="key">The key of the data to get.</param>
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        /// <summary>
        /// Clears data from the context of the tree. The data will no longer be available to any node in the tree.
        /// </summary>
        /// <param name="key">The key of the data to clear.</param>
        /// <returns>True if the data has been successfully cleaned, false otherwise.</returns>
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }
}
