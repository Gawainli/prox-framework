using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.Runtime.Settings;
using ProxFramework.StateMachine;

namespace GameName.Runtime
{
    public class StateInitPackage : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            AssetModule.Initialize();
            await AssetModule.InitPackage(AssetModule.DefaultPkgName).ToUniTask();
            await AssetModule.InitPackage(AssetModule.DefaultRawPkgName).ToUniTask();

            foreach (var packageName in SettingsUtil.GlobalSettings.assetSettings.packageNames)
            {
                await AssetModule.InitPackage(packageName).ToUniTask();
            }

            ChangeState<StateUpdateVersion>();
        }

        public override void Exit()
        {
        }
    }
}