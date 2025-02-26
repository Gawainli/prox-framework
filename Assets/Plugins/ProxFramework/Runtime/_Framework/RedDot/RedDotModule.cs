using System.Collections.Generic;

namespace ProxFramework.RedDot
{
    public static class RedDotModule
    {
        private static readonly Dictionary<string, RedDotNode> _nodes = new();
        private static HashSet<RedDotNode> _dirtyNodes = new();
        private static List<RedDotNode> _updateList = new();

        public static void Initialize(RedDotConfig config)
        {
            foreach (var nodeConfig in config.nodeConfigs)
            {
                var node = GetOrCreateNode(nodeConfig.nodeId);
                node.MergeType = nodeConfig.mergeType;
                node.Lifetime = nodeConfig.lifetime;

                foreach (var sourceId in nodeConfig.sourceIds)
                {
                    var sourceNode = GetOrCreateNode(sourceId);
                    node.AddSource(sourceNode);
                }
            }
        }
        
        public static void Tick()
        {
            ProcessNodesLifeTime();
            ProcessDirtyNodes();
        }
        
        public static void Shutdown()
        {
            _nodes.Clear();
            _dirtyNodes.Clear();
            _updateList.Clear();
        }

        public static void MarkDirty(RedDotNode node)
        {
            if (node == null) return;

            _dirtyNodes.Add(node);
        }
        
        private static void ProcessNodesLifeTime()
        {
            foreach (var node in _nodes.Values)
            {
                node.TickLifeTime();
            }
        }

        private static void ProcessDirtyNodes()
        {
            if (_dirtyNodes.Count == 0) return;

            // 将脏节点转换为处理队列
            _updateList.Clear();
            _updateList.AddRange(_dirtyNodes);
            _dirtyNodes.Clear();

            // 拓扑排序确保处理顺序正确
            TopologicalSort(_updateList);

            // 批量处理更新
            foreach (var node in _updateList)
            {
                node.UpdateCount();
            }
        }

        private static void TopologicalSort(List<RedDotNode> nodes)
        {
            var visited = new HashSet<RedDotNode>();
            var sorted = new List<RedDotNode>();

            foreach (var node in nodes)
            {
                Visit(node, visited, sorted);
            }

            nodes.Clear();
            nodes.AddRange(sorted);
        }

        private static void Visit(RedDotNode node, HashSet<RedDotNode> visited, List<RedDotNode> sorted)
        {
            if (!visited.Add(node)) return;

            foreach (var child in node.Sources)
            {
                Visit(child, visited, sorted);
            }

            sorted.Add(node);
        }

        public static RedDotNode GetOrCreateNode(string id)
        {
            if (!_nodes.TryGetValue(id, out var node))
            {
                node = new RedDotNode(id);
                _nodes[id] = node;
            }

            return node;
        }
        
        public static void ClearAllRedDot()
        {
            foreach (var node in _nodes.Values)
            {
                node.SetCountDirect(0);
            }
        }

        public static void DebugPrintStatus()
        {
            foreach (var node in _nodes.Values)
            {
                PLogger.Info($"{node.Id}: Count={node.Count} Dirty={_dirtyNodes.Contains(node)}");
            }
        }
    }
}