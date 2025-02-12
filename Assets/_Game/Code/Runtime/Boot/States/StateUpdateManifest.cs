using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using YooAsset;

namespace Prox.GameName.Runtime
{
    public class StateUpdateManifest : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            var succeed = await UpdateManifest();
            if (succeed)
            {
                ChangeState<StateCreateDownloader>();
            }
        }

        public override void Exit()
        {
        }

        private async UniTask<bool> UpdateManifest()
        {
            foreach (var pkg in AssetModule.GetAllPackages())
            {
                var pkgVersion = fsm.Blackboard.GetStringValue(pkg.PackageName);
                var op = await AssetModule.UpdatePackageManifestAsync(pkgVersion, 60, pkg.PackageName);
                if (op.Status == EOperationStatus.Failed)
                {
                    PLogger.Error($"Package {pkg.PackageName} UpdateVersion Failed : {op.Error}");
                    return false;
                }
            }

            return true;
        }
    }
}