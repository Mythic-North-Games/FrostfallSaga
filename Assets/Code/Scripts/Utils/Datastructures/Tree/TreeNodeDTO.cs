using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils.DataStructures.TreeNode
{
    [Serializable]
    public class TreeNodeDTO<T>
    {
        public string ID;
        public T Data;
        public string ParentID;
        public List<string> ChildrenIDs = new();
    }
}
