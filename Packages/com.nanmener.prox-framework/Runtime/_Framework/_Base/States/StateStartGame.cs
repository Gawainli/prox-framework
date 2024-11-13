using ProxFramework.Asset;
using ProxFramework.Logger;
using ProxFramework.Module;
using ProxFramework.Scene;
using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StateStartGame : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            var all = AssetModule.assetPkg.GetAssetInfos("");
            foreach (var info in all)
            {
                LogModule.Info(info.AssetPath);
            }
            await SceneModule.ChangeSceneAsync("Assets/_Game/_Scenes/S_Main");
            
        }

        public override void Exit()
        {
        }
    }
}