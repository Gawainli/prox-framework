namespace ProxFramework.Pool
{
    public interface IPoolObj
    {
        void Init(params System.Object[] userDatas);
        void Reset();
        AssetObjectPool Pool { get; set; }
    }
}