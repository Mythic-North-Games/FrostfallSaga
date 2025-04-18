using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Utils.Trees
{
    [Serializable]
    public class TreeNode<T>
    {
        public List<TreeNode<T>> Children = new();
        [SerializeReference] public T Data;
        [NonSerialized] public TreeNode<T> Parent;

        public TreeNode()
        {
            Children = new List<TreeNode<T>>();
        }

        public TreeNode(T data)
        {
            Data = data;
            Children = new List<TreeNode<T>>();
        }

        /// <summary>
        ///     Adds a child to the tree node.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(TreeNode<T> child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        ///     Removes a child from the tree node. If not in direct children, it will search in the children's children.
        /// </summary>
        /// <param name="child">The child to remove.</param>
        /// <returns>True if the child was removed, false otherwise.</returns>
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

        public bool HasChildren()
        {
            return Children.Count > 0;
        }

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