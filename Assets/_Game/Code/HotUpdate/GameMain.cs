using System;
using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Scene;

namespace GameName.Core
{
    public static class GameMain
    {
        public static async void StartGame()
        {
            try
            {
                await DataSystem.Initialize();
                PLogger.Info($"DataSystem initialized. Item table count:{DataSystem.Tables.TbItem.DataList.Count}");
                
                PLogger.Info("start game. change scene to Main");
                SceneModule.ChangeTopSceneAsync("Assets/_Game/Scenes/Main.unity").Forget();
            }
            catch (Exception e)
            {
                PLogger.Error(e.ToString());
            }
        }
    }
}