using System.Collections.Generic;
using UnityEngine;

namespace ProxFramework.RedDot
{
    [CreateAssetMenu(fileName = "RedDotConfig", menuName = "Prox/RedDotConfig", order = 0)]
    public class RedDotConfig : ScriptableObject
    {
        public List<RedDotData> nodeConfigs = new();

        public void CheckForUselessNodes()
        {
            var adjacencyList = new Dictionary<string, List<string>>();

            // 构建邻接表（source -> target）
            foreach (var config in nodeConfigs)
            {
                foreach (var sourceId in config.sourceIds)
                {
                    if (!adjacencyList.ContainsKey(sourceId))
                    {
                        adjacencyList[sourceId] = new List<string>();
                    }

                    adjacencyList[sourceId].Add(config.nodeId);
                }
            }

            var visited = new HashSet<string>();
            var recursionStack = new HashSet<string>();

            foreach (var node in adjacencyList.Keys)
            {
                DFS(node, adjacencyList, visited, recursionStack);
            }

            var count = 0;
            foreach (var config in nodeConfigs)
            {
                if (!visited.Contains(config.nodeId))
                {
                    PLogger.Warning($"Node [{config.nodeId}] is not reachable from any other node!");
                    count++;
                }
            }

            if (count == 0)
            {
                PLogger.Info("No useless nodes detected.");
            }
            else
            {
                PLogger.Warning($"Found {count} useless nodes.");
            }
        }

        public void CheckForCycles()
        {
            var adjacencyList = new Dictionary<string, List<string>>();

            // 构建邻接表（source -> target）
            foreach (var config in nodeConfigs)
            {
                foreach (var sourceId in config.sourceIds)
                {
                    if (!adjacencyList.ContainsKey(sourceId))
                    {
                        adjacencyList[sourceId] = new List<string>();
                    }

                    adjacencyList[sourceId].Add(config.nodeId);
                }
            }

            var visited = new HashSet<string>();
            var recursionStack = new HashSet<string>();

            foreach (var node in adjacencyList.Keys)
            {
                if (DFS(node, adjacencyList, visited, recursionStack))
                {
                    PLogger.Error("Cycle detected in red dot configuration!");
                    return;
                }
            }

            PLogger.Info("No cycles detected.");
        }

        private static bool DFS(string current,
            Dictionary<string, List<string>> graph,
            HashSet<string> visited,
            HashSet<string> stack)
        {
            if (stack.Contains(current)) return true;
            if (!visited.Add(current)) return false;

            stack.Add(current);

            if (graph.TryGetValue(current, out var neighbors))
            {
                foreach (var neighbor in neighbors)
                {
                    if (DFS(neighbor, graph, visited, stack))
                    {
                        PLogger.Error($"Find Cycle: [{current}] to [{neighbor}]");
                        return true;
                    }
                }
            }

            stack.Remove(current);
            return false;
        }
    }
}