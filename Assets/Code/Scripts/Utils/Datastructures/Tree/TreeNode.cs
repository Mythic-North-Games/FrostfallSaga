using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils.DataStructures.TreeNode
{
    [Serializable]
    public class TreeNode<T>
    {
        public string ID;
        public T Data;
        [NonSerialized] public TreeNode<T> Parent;
        [NonSerialized] public List<TreeNode<T>> Children = new();

        public TreeNode() { }

        public TreeNode(string id, T data)
        {
            ID = id;
            Data = data;
        }

        public void AddChild(TreeNode<T> child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public bool RemoveChild(TreeNode<T> child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
                return true;
            }

            foreach (TreeNode<T> node in Children)
                if (node.RemoveChild(child))
                    return true;

            return false;
        }

        public bool HasChildren() => Children.Count > 0;

        public static TreeNode<T> FindChild(TreeNode<T> root, T data)
        {
            if (root.Data.Equals(data)) return root;

            foreach (TreeNode<T> child in root.Children)
            {
                TreeNode<T> found = FindChild(child, data);
                if (found != null) return found;
            }

            return null;
        }
    }
}
