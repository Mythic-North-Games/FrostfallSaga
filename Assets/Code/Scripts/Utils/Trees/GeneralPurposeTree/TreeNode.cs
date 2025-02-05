using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils.Trees
{
    public class TreeNode<T>
    {
        private T _data;
        private List<TreeNode<T>> _children;

        public TreeNode(T data)
        {
            _data = data;
            _children = new List<TreeNode<T>>();
        }

        /// <summary>
        /// Adds a child to the tree node.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(TreeNode<T> child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Removes a child from the tree node. If not in direct children, it will search in the children's children.
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

            foreach (var node in _children)
            {
                if (node.RemoveChild(child))
                {
                    return true;
                }
            }

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
    }
}