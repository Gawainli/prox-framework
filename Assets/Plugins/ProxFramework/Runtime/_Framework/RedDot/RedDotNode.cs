using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProxFramework.RedDot
{
    public class RedDotNode
    {
        public string Id { get; private set; }
        public int Count { get; private set; }
        public bool IsActive => Count > 0;
        public RedDotMergeType MergeType { get; set; }
        public List<RedDotNode> Sources { get; } = new List<RedDotNode>();
        public float Lifetime { get; set; }

        private float _activationTime;
        private bool _isExpired;

        public event Action<RedDotNode> OnStateChanged;

        public RedDotNode(string id)
        {
            Id = id;
        }

        public void AddSource(RedDotNode source)
        {
            if (Sources.Contains(source)) return;
            Sources.Add(source);
            source.OnStateChanged += HandleSourceUpdate;
            RedDotModule.MarkDirty(this);
        }

        private void HandleSourceUpdate(RedDotNode rdNode)
        {
            UpdateCount();
        }

        public void SetCountDirect(int count)
        {
            if (Count == count) return;
        
            Count = count;
            RedDotModule.MarkDirty(this);
            TriggerStateChanged();
            ResetLifeTime();
        }

        public void UpdateCount()
        {
            var newCount = MergeType switch
            {
                RedDotMergeType.Sum => Sources.Sum(s => s.Count),
                RedDotMergeType.Max => Sources.Any() ? Sources.Max(s => s.Count) : 0,
                RedDotMergeType.Override => Sources.LastOrDefault()?.Count ?? 0,
                _ => 0
            };

            if (newCount != Count)
            {
                Count = newCount;
                TriggerStateChanged();
            }
        }

        private void TriggerStateChanged()
        {
            OnStateChanged?.Invoke(this);
        }

        private void ResetLifeTime()
        {
            if (Lifetime <= 0 || !_isExpired)
            {
                return;
            }
            _isExpired = false;
            _activationTime = Time.time;
        }
        
        public void TickLifeTime()
        {
            if (Lifetime <= 0 || _isExpired)
            {
                return;
            }
            
            if (Time.time - _activationTime > Lifetime)
            {
                SetCountDirect(0);
                _isExpired = true;
            }
        }
    }
}