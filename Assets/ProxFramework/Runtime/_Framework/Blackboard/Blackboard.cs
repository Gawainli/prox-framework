using System;
using System.Collections.Generic;
using ProxFramework.Logger;

namespace ProxFramework.Blackboard
{
    public class Blackboard
    {
        public Action onFlush;

        private readonly bool _useBuffer = false;

        private readonly Dictionary<string, BlackboardValue> _valueByName = new Dictionary<string, BlackboardValue>();

        private readonly Dictionary<string, BlackboardValue> _valueByNameBuffer =
            new Dictionary<string, BlackboardValue>();

        public Blackboard(bool useBuffer = false)
        {
            _useBuffer = useBuffer;
        }

        public bool TryAddNumberValue(string key, float value)
        {
            if (_useBuffer)
            {
                return _valueByNameBuffer.TryAdd(key,
                    new BlackboardValue { NumberValue = value, Type = ValueType.Number });
            }

            return _valueByName.TryAdd(key, new BlackboardValue { NumberValue = value, Type = ValueType.Number });
        }
        
        public bool HasKey(string key)
        {
            return _valueByName.ContainsKey(key);
        }

        public void SetNumberValue(string key, float value)
        {
            if (_useBuffer)
            {
                _valueByNameBuffer[key].NumberValue = value;
            }
            else
            {
                _valueByName[key].NumberValue = value;
            }
        }

        public float GetNumberValue(string key)
        {
            if (_valueByName.TryGetValue(key, out var blackboardValue) && blackboardValue.Type == ValueType.Number)
            {
                return blackboardValue.NumberValue;
            }

            // LogModule.Warning($"Blackboard.GetNumberValue: key {key} not found, return 0");
            return 0;
        }

        public bool TryAddObjectValue(string key, object value)
        {
            if (_useBuffer)
            {
                return _valueByNameBuffer.TryAdd(key,
                    new BlackboardValue { ObjectValue = value, Type = ValueType.Object });
            }

            return _valueByName.TryAdd(key, new BlackboardValue { ObjectValue = value, Type = ValueType.Object });
        }

        public void SetObjectValue(string key, object value)
        {
            if (_useBuffer)
            {
                _valueByNameBuffer[key].ObjectValue = value;
            }
            else
            {
                _valueByName[key].ObjectValue = value;
            }
        }

        public object GetObjectValue(string key)
        {
            if (_valueByName.TryGetValue(key, out var blackboardValue) && blackboardValue.Type == ValueType.Object)
            {
                return blackboardValue.ObjectValue;
            }

            // LogModule.Error($"Blackboard.GetObjectValue: key {key} not found, return null");
            return null;
        }

        public T GetObjectValue<T>(string key)
        {
            return (T)GetObjectValue(key);
        }

        public void RemoveValue(string key)
        {
            _valueByName.Remove(key);
            if (_useBuffer)
            {
                _valueByNameBuffer.Remove(key);
            }
        }

        public void Flush()
        {
            if (!_useBuffer)
            {
                return;
            }

            foreach (var pair in _valueByNameBuffer)
            {
                _valueByName[pair.Key] = pair.Value;
            }

            onFlush?.Invoke();

            _valueByNameBuffer.Clear();
        }

        public void Clear()
        {
            _valueByName.Clear();
            _valueByNameBuffer.Clear();
        }

        public override string ToString()
        {
            var str = $"Blackboard: {_valueByName.Count} items\n";
            foreach (var pair in _valueByName)
            {
                var value = pair.Value.Type == ValueType.Number
                    ? pair.Value.NumberValue.ToString()
                    : pair.Value.ObjectValue.GetType().ToString();
                str += $"{pair.Key}:{value}\n";
            }

            return str;
        }
    }
}