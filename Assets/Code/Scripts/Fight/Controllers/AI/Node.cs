using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.AI
{
    /// <summary>
    /// Enumeration representing the possible states of a node in the behavior tree.
    /// </summary>
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    /// <summary>
    /// Base class for nodes in a behavior tree used for AI decision-making.
    /// A node can have children and a parent, and it manages an execution state (RUNNING, SUCCESS, FAILURE).
    /// </summary>
    public class Node
    {
        /// <summary>
        /// The current state of the node (RUNNING, SUCCESS, FAILURE).
        /// </summary>
        protected NodeState state;

        /// <summary>
        /// Reference to the parent node. If null, the node is the root of the tree.
        /// </summary>
        public Node parent;

        /// <summary>
        /// List of children nodes associated with this node.
        /// </summary>
        protected List<Node> children;

        /// <summary>
        /// Dictionary for storing context data for this node.
        /// Used to pass information between nodes via key-value pairs.
        /// </summary>
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        /// <summary>
        /// Default constructor. Initializes a node without any parent or children.
        /// </summary>
        public Node()
        {
            parent = null;
        }
        
        /// <summary>
        /// Constructor for initializing a node with a list of child nodes.
        /// </summary>
        /// <param name="children">List of child nodes to attach to this node.</param>
        public Node(List<Node> children)
        {

            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        /// <summary>
        /// Attaches a child node to this node, setting this node as the parent of the child.
        /// </summary>
        /// <param name="node">The node to attach as a child.</param>
        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        /// <summary>
        /// Evaluates the state of the node. This method is virtual and is meant to be overridden
        /// by derived classes to provide specific evaluation logic.
        /// </summary>
        /// <returns>The state of the node (SUCCESS, FAILURE, or RUNNING).</returns>
        protected virtual NodeState Evaluate() => NodeState.FAILURE;

        /// <summary>
        /// Sets a piece of data in the node's context, associating it with the given key.
        /// </summary>
        /// <param name="key">The key to associate the data with.</param>
        /// <param name="value">The value to associate with the key.</param>
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        /// <summary>
        /// Retrieves data from the node's context based on the provided key.
        /// If the key does not exist in the current node's context, the method searches the parent nodes.
        /// </summary>
        /// <param name="key">The key for the data to retrieve.</param>
        /// <returns>The value associated with the key, or null if the key is not found.</returns>
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value)) return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null) return value;
                node = node.parent;
            }
            return null;
        }

        /// <summary>
        /// Clears data associated with the given key from the node's context or its parent nodes.
        /// </summary>
        /// <param name="key">The key for the data to remove.</param>
        /// <returns>True if the data was successfully removed, false otherwise.</returns>
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
                if (cleared) return true;
                node = node.parent;
            }
            return false;
        }
    }
}
