using System.Collections.Generic;

namespace FrostfallSaga.Utils.DataStructures.GraphNode
{
    public static class GraphNodeSerializer
    {
        public static List<GraphNodeDTO<T>> SerializeGraph<T>(List<GraphNode<T>> nodes)
        {
            var dtos = new List<GraphNodeDTO<T>>();
            var visited = new HashSet<string>();

            foreach (var node in nodes)
            {
                SerializeRecursive(node, dtos, visited);
            }

            return dtos;
        }

        private static void SerializeRecursive<T>(GraphNode<T> node, List<GraphNodeDTO<T>> dtos, HashSet<string> visited)
        {
            if (node == null || visited.Contains(node.ID)) return;

            visited.Add(node.ID);

            var dto = new GraphNodeDTO<T>
            {
                ID = node.ID,
                Data = node.Data,
                ChildrenIDs = node.Children.ConvertAll(child => child.ID),
                ParentIDs = node.Parents.ConvertAll(parent => parent.ID)
            };

            dtos.Add(dto);

            foreach (var child in node.Children)
            {
                SerializeRecursive(child, dtos, visited);
            }
        }

        public static List<GraphNode<T>> DeserializeGraph<T>(List<GraphNodeDTO<T>> dtos)
        {
            var nodeMap = new Dictionary<string, GraphNode<T>>();

            // Step 1: Create all nodes
            foreach (var dto in dtos)
            {
                nodeMap[dto.ID] = new GraphNode<T>(dto.ID, dto.Data);
            }

            // Step 2: Link relationships
            foreach (var dto in dtos)
            {
                var node = nodeMap[dto.ID];

                node.Parents = dto.ParentIDs.ConvertAll(id => nodeMap[id]);
                node.Children = dto.ChildrenIDs.ConvertAll(id => nodeMap[id]);
            }

            return new List<GraphNode<T>>(nodeMap.Values);
        }
    }
}
