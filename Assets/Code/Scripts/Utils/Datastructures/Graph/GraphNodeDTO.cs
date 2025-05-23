using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils.DataStructures.GraphNode
{
    [Serializable]
    public class GraphNodeDTO<T>
    {
        public string ID;
        public T Data;
        public List<string> ParentIDs = new();
        public List<string> ChildrenIDs = new();
    }
}