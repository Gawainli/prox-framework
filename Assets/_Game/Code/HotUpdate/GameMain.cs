using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Scene;

namespace GameName.GameMain
{
    public static class GameMain
    {
        public static void StartGame()
        {
            PLogger.Info("start game. change scene to Main");
            SceneModule.ChangeTopSceneAsync("Assets/_Game/Scenes/Main.unity").Forget();
        }
    }
}