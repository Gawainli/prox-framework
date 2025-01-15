using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using YooAsset;

namespace GameName.Runtime
{
    public class StateUpdateVersion : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
            {
                //todo:do some work for no internet
            }

            var isAllSucceed = await UpdateVersion();
            if (isAllSucceed)
            {
                ChangeState<StateUpdateManifest>();
            }
        }

        private async UniTask<bool> UpdateVersion()
        {
            foreach (var pkg in AssetModule.GetAllPackages())
            {
                var op = AssetModule.UpdatePackageVersionAsync(true, 60, pkg.PackageName);
                await op.ToUniTask();
                if (op.Status == EOperationStatus.Failed)
                {
                    PLogger.Error($"Package {pkg.PackageName} UpdateVersion Failed : {op.Error}");
                    return false;
                }
            }

            return true;
        }

        public override void Exit()
        {
        }
    }
}