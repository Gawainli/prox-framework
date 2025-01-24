using System;
using System.Collections.Generic;

namespace ProxFramework.Blackboard
{
    public partial class Blackboard
    {
        public string name;
        private readonly int _capacity;
        private readonly Dictionary<string, BbItem> _mapKeyNameToBbItems = new();
        private Queue<BbItem> _queueBbItems;

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

        public bool HasKey(string key)
        {
            return _mapKeyNameToBbItems.ContainsKey(key);
        }

        public void SetNumberValue(string key, float value)
        {
            if (_mapKeyNameToBbItems.TryGetValue(key, out var outItem))
            {
                outItem.NumberValue = value;
            }
            else
            {
                var bbItem = GetBbItem();
                bbItem.NumberValue = value;
                _mapKeyNameToBbItems.Add(key, bbItem);
            }
        }

        public float GetNumberValue(string key)
        {
            if (_mapKeyNameToBbItems.TryGetValue(key, out var outItem))
            {
                return outItem.NumberValue;
            }

            return 0;
        }

        public void SetObjectValue(string key, object value)
        {
            if (_mapKeyNameToBbItems.TryGetValue(key, out var outItem))
            {
                outItem.ObjectValue = value;
            }
            else
            {
                var bbItem = GetBbItem();
                bbItem.ObjectValue = value;
                _mapKeyNameToBbItems.Add(key, bbItem);
            }
        }

        public object GetObjectValue(string key)
        {
            if (_mapKeyNameToBbItems.TryGetValue(key, out var outItem))
            {
                return outItem.ObjectValue;
            }

            return null;
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
            var str = $"Blackboard {name}: {_mapKeyNameToBbItems.Count} items\n";
            foreach (var pair in _mapKeyNameToBbItems)
            {
                str += $"{pair.Key} = {pair.Value}\n";
            }

            return str;
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