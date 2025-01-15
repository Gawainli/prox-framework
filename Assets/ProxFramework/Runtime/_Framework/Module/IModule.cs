namespace ProxFramework.Module
{
    public interface IModule
    {
        // bool IsInitialized { get; }
        void Initialize(object userData = null);

        void Tick(float deltaTime, float unscaledDeltaTime);

        void Shutdown();
        
        int Priority { get; set; }
        
        bool Initialized { get; set; }
    }
}