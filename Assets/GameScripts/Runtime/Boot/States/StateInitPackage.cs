using ProxFramework.Asset;
using ProxFramework.Module;
using ProxFramework.StateMachine;
using YooAsset;

namespace ProxFramework.Base
{
    public class StateInitPackage : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            // var module = ModuleCore.GetModule<AssetModule>();
            // var succeed = await module.InitPkgAsync();
            // if (succeed)
            // {
                // if (SettingUtils.playMode == EPlayMode.EditorSimulateMode || SettingUtils.playMode == EPlayMode.OfflinePlayMode || SettingUtils.playMode == EPlayMode.WebPlayMode)
                // {
                //     ChangeState<StateStartGame>();
                // }
                // else
                // {
                //     ChangeState<StateUpdateVersion>();
                // }
            // }
        }

        public override void Exit()
        {
        }
    }
}