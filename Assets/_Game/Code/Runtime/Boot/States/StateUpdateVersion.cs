using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using YooAsset;

namespace Prox.GameName.Runtime
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
                var op = await AssetModule.UpdatePackageVersionAsync(true, 60, pkg.PackageName);
                if (op.Status == EOperationStatus.Failed)
                {
                    PLogger.Error($"Package {pkg.PackageName} UpdateVersion Failed : {op.Error}");
                    return false;
                }

                fsm.Blackboard.SetStringValue(op.PackageName, op.PackageVersion);
            }

            return true;
        }

        public override void Exit()
        {
        }
    }
}