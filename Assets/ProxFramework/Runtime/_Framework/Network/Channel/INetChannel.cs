namespace ProxFramework.Network
{
    public interface INetChannel
    {
        bool Connected { get; }
        void Update(float unscaledDeltaTime);
        void SendPkg(INetPackage pkg);
        INetPackage PickPkg();
    }
}