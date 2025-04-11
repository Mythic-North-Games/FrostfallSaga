using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Utils.Trees
{
    [Serializable]
    public class GraphNode<T>
    {
        public T Data;
        public List<GraphNode<T>> Parents = new List<GraphNode<T>>();
        public List<GraphNode<T>> Children = new List<GraphNode<T>>();
    }
}
