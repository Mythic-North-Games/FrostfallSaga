using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.AI
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }
    public class Node
    {
        protected NodeState state;
        public Node parent;
        protected List<Node> children;
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();
        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        protected virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value)
        {
            _dataContext[key] = value;

        }

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
