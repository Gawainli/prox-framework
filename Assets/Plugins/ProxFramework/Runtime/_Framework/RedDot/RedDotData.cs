using System;
using System.Collections.Generic;

namespace ProxFramework.RedDot
{
    public enum RedDotMergeType
    {
        Sum,
        Max,
        Override
    }
    
    [Serializable]
    public class RedDotData
    {
        public string nodeId;
        public RedDotMergeType mergeType;
        public float lifetime;
        public List<string> sourceIds = new();
    }
}