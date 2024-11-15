namespace ProxFramework.Blackboard
{
    public enum ValueType
    {
        Number,
        Object,
    }

    //防止数值作为黑板值被装箱拆箱
    public class BlackboardValue
    {
        public ValueType Type { get; set; }
        public float NumberValue { get; set; }
        public object ObjectValue { get; set; }
    }
}