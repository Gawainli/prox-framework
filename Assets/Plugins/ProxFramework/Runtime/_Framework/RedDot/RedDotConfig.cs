using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProxFramework.RedDot
{
    [CreateAssetMenu(fileName = "RedDotConfig", menuName = "Prox/RedDotConfig", order = 0)]
    public class RedDotConfig : ScriptableObject
    {
        public List<RedDotData> nodeConfigs = new();

#if UNITY_EDITOR
        [CustomEditor(typeof(RedDotConfig))]
        public class RedDotConfigEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Check for Cycles"))
                {
                    ((RedDotConfig)target).CheckForCycles();
                }
            }
        }
#endif

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
                    Debug.LogError("Cycle detected in red dot configuration!");
                    return;
                }
            }

            Debug.Log("No cycles detected.");
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
                        return true;
                    }
                }
            }

            stack.Remove(current);
            return false;
        }
    }
}