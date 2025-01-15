using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.Runtime.Settings;
using ProxFramework.StateMachine;
using YooAsset;

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
            var status = await AssetModule.InitPackage(AssetModule.DefaultPkgName);
            if (status == EOperationStatus.Failed)
            {
                PLogger.Error($"package:{AssetModule.DefaultPkgName} init failed");
                return;
            }

            status = await AssetModule.InitPackage(AssetModule.DefaultRawPkgName);
            if (status == EOperationStatus.Failed)
            {
                PLogger.Error($"package:{AssetModule.DefaultRawPkgName} init failed");
                return;
            }

            foreach (var packageName in SettingsUtil.GlobalSettings.assetSettings.packageNames)
            {
                status = await AssetModule.InitPackage(packageName);
                if (status == EOperationStatus.Failed)
                {
                    PLogger.Error($"package:{packageName} init failed");
                    return;
                }
            }

            ChangeState<StateUpdateVersion>();
        }

        public override void Exit()
        {
        }
    }
}