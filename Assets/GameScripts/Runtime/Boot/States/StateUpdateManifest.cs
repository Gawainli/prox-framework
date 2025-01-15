using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using YooAsset;

namespace GameName.Runtime
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
                var op = AssetModule.UpdatePackageManifestAsync(pkg.PackageName);
                await op.ToUniTask();
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