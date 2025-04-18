using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Utils.Trees
{
    [Serializable]
    public class GraphNode<T>
    {
        public T Data;
        [NonSerialized] public List<GraphNode<T>> Parents = new();
        [SerializeReference] public List<GraphNode<T>> Children = new();

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
