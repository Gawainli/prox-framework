namespace GameName.Base.Singleton
{
    public interface IGameSingleton
    {
        void Init(System.Object[] param);
        void Update();
        void Release();
    }
}