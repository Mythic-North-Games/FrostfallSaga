using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils.DataStructures.GraphNode
{
    [Serializable]
    public class GraphNode<T>
    {
        public string ID;
        public T Data;
        [NonSerialized] public List<GraphNode<T>> Parents = new();
        [NonSerialized] public List<GraphNode<T>> Children = new();

        public GraphNode(string id, T data)
        {
            ID = id;
            Data = data;
        }

        public static GraphNode<T> FindInGraph(GraphNode<T> root, T data)
        {
            if (root.Data.Equals(data)) return root;

            foreach (GraphNode<T> child in root.Children)
            {
                GraphNode<T> found = FindInGraph(child, data);
                if (found != null) return found;
            }

            return null;
        }
    }
}
