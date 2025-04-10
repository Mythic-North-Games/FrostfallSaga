using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils.Trees
{
    [Serializable]
    public class TreeNode<T>
    {
        private List<TreeNode<T>> _children = new();
        private T _data;
        private TreeNode<T> _parent;

        public TreeNode(T data)
        {
            _data = data;
            _children = new List<TreeNode<T>>();
        }

        /// <summary>
        ///     Adds a child to the tree node.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(TreeNode<T> child)
        {
            _children.Add(child);
            child._parent = this;
        }

        /// <summary>
        ///     Removes a child from the tree node. If not in direct children, it will search in the children's children.
        /// </summary>
        /// <param name="child">The child to remove.</param>
        /// <returns>True if the child was removed, false otherwise.</returns>
        public bool RemoveChild(TreeNode<T> child)
        {
            if (_children.Contains(child))
            {
                _children.Remove(child);
                return true;
            }

            foreach (TreeNode<T> node in _children)
                if (node.RemoveChild(child))
                    return true;

            return false;
        }

        public T GetData()
        {
            return _data;
        }

        public void SetData(T data)
        {
            _data = data;
        }

        public List<TreeNode<T>> GetChildren()
        {
            return _children;
        }

        public void SetChildren(List<TreeNode<T>> children)
        {
            _children = children;
        }

        public bool HasChildren()
        {
            return _children.Count > 0;
        }

        public TreeNode<T> GetParent()
        {
            return _parent;
        }

        public static TreeNode<T> FindChild(TreeNode<T> root, T data)
        {
            if (root.GetData().Equals(data)) return root;

            foreach (TreeNode<T> child in root.GetChildren())
            {
                TreeNode<T> found = FindChild(child, data);
                if (found != null) return found;
            }

            return null;
        }
    }
}