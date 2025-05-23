using System.Collections.Generic;

namespace FrostfallSaga.Utils.DataStructures.TreeNode
{
    public static class TreeNodeSerializer
    {
        public static List<TreeNodeDTO<T>> SerializeTree<T>(TreeNode<T> root)
        {
            var dtos = new List<TreeNodeDTO<T>>();
            var visited = new HashSet<string>();
            SerializeRecursive(root, dtos, visited);
            return dtos;
        }

        private static void SerializeRecursive<T>(TreeNode<T> node, List<TreeNodeDTO<T>> dtos, HashSet<string> visited)
        {
            if (node == null || visited.Contains(node.ID)) return;

            visited.Add(node.ID);

            var dto = new TreeNodeDTO<T>
            {
                ID = node.ID,
                Data = node.Data,
                ParentID = node.Parent?.ID,
                ChildrenIDs = node.Children.ConvertAll(child => child.ID)
            };

            dtos.Add(dto);

            foreach (var child in node.Children)
            {
                SerializeRecursive(child, dtos, visited);
            }
        }

        public static TreeNode<T> DeserializeTree<T>(List<TreeNodeDTO<T>> dtos)
        {
            var nodeMap = new Dictionary<string, TreeNode<T>>();

            // Step 1: Create nodes
            foreach (var dto in dtos)
            {
                nodeMap[dto.ID] = new TreeNode<T>(dto.ID, dto.Data);
            }

            // Step 2: Link children and set parent based on ParentID
            foreach (var dto in dtos)
            {
                var node = nodeMap[dto.ID];

                if (!string.IsNullOrEmpty(dto.ParentID) && nodeMap.TryGetValue(dto.ParentID, out var parent))
                {
                    node.Parent = parent;
                    parent.Children.Add(node);
                }
            }

            // Step 3: Return the root node (node with no parent)
            foreach (var node in nodeMap.Values)
            {
                if (node.Parent == null)
                    return node;
            }

            return null;
        }
    }
}
