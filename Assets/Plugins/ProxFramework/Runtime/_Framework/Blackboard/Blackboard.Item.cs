namespace ProxFramework.Blackboard
{
    public partial class Blackboard
    {
        private enum ValueType
        {
            None,
            Number,
            Object,
        }

        //防止数值作为黑板值被装箱拆箱
        private sealed class BbItem
        {
            private ValueType _valueType;

            private float _numberValue;

            public float NumberValue
            {
                get
                {
                    if (_valueType != ValueType.Number)
                    {
                        throw new System.Exception("Value is not a number");
                    }

                    return _numberValue;
                }
                set
                {
                    _valueType = ValueType.Number;
                    _numberValue = value;
                }
            }

            private object _objectValue;
            public object ObjectValue
            {
                get
                {
                    if (_valueType != ValueType.Object)
                    {
                        throw new System.Exception("Value is not an object");
                    }

                    return _objectValue;
                }
                set
                {
                    _valueType = ValueType.Object;
                    _objectValue = value;
                }
            }

            public void Clear()
            {
                _valueType = ValueType.None;
                NumberValue = 0;
                ObjectValue = null;
            }

            public override string ToString()
            {
                if (_valueType == ValueType.Number)
                {
                    return NumberValue.ToString();
                }
                else if (_valueType == ValueType.Object)
                {
                    return ObjectValue.ToString();
                }

                return "None";
            }
        }
    }
}