using System;
using System.Collections.Generic;
using System.Text;

namespace ProxFramework.Blackboard
{
    public partial class Blackboard
    {
        public string name;
        private readonly int _capacity;
        private readonly Dictionary<string, BbItem> _mapKeyNameToBbItems = new();
        private readonly Queue<BbItem> _queueBbItems;

        private static List<WeakReference<Blackboard>> _referenceBoards = new();

        public Blackboard(string name, int capacity = 100)
        {
            this.name = name;
            _capacity = capacity;
            _queueBbItems = new Queue<BbItem>(_capacity);

            for (var i = 0; i < _capacity; i++)
            {
                _queueBbItems.Enqueue(new BbItem());
            }

            _referenceBoards.Add(new WeakReference<Blackboard>(this));
        }

        private BbItem GetBbItem()
        {
            if (_queueBbItems.Count == 0)
            {
                for (var i = 0; i < _capacity; i++)
                {
                    _queueBbItems.Enqueue(new BbItem());
                }
            }

            return _queueBbItems.Dequeue();
        }

        private void SetValue(string key, Action<BbItem> setValueAction)
        {
            if (_mapKeyNameToBbItems.TryGetValue(key, out var outItem))
            {
                setValueAction(outItem);
            }
            else
            {
                var bbItem = GetBbItem();
                setValueAction(bbItem);
                _mapKeyNameToBbItems.Add(key, bbItem);
            }
        }

        public bool HasKey(string key)
        {
            return _mapKeyNameToBbItems.ContainsKey(key);
        }

        public void SetNumberValue(string key, float value)
        {
            SetValue(key, item => item.NumberValue = value);
        }

        public float GetNumberValue(string key)
        {
            return _mapKeyNameToBbItems.TryGetValue(key, out var outItem) ? outItem.NumberValue : 0;
        }

        public void SetObjectValue(string key, object value)
        {
            SetValue(key, item => item.ObjectValue = value);
        }

        public object GetObjectValue(string key)
        {
            return _mapKeyNameToBbItems.TryGetValue(key, out var outItem) ? outItem.ObjectValue : null;
        }

        public void SetStringValue(string key, string value)
        {
            SetObjectValue(key, value);
        }

        public string GetStringValue(string key)
        {
            return GetObjectValue<string>(key);
        }

        public T GetObjectValue<T>(string key)
        {
            return (T)GetObjectValue(key);
        }

        public void RemoveValue(string key)
        {
            if (!_mapKeyNameToBbItems.Remove(key, out var bbItem)) return;
            bbItem.Clear();
            _queueBbItems.Enqueue(bbItem);
        }

        public void ClearAll()
        {
            foreach (var pair in _mapKeyNameToBbItems)
            {
                pair.Value.Clear();
                _queueBbItems.Enqueue(pair.Value);
            }

            _mapKeyNameToBbItems.Clear();
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"Blackboard {name}: {_mapKeyNameToBbItems.Count} items\n");
            foreach (var pair in _mapKeyNameToBbItems)
            {
                sb.AppendLine($"{pair.Key} = {pair.Value}");
            }

            return sb.ToString();
        }

        public static IEnumerable<Blackboard> GetAllBoards()
        {
            List<Blackboard> aliveBoards = new();
            foreach (var reference in _referenceBoards)
            {
                if (reference.TryGetTarget(out var board))
                {
                    aliveBoards.Add(board);
                }
            }

            return aliveBoards;
        }
    }
}