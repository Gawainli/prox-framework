using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.StateMachine;

namespace GameName.Runtime
{
    public class StatePatchDone : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            foreach (var pkg in AssetModule.hostPackages)
            {
                await AssetModule.ClearUnusedCacheFilesAsync(pkg.PackageName).ToUniTask();
            }
            AssetModule.hostPackages.Clear();

#if ENABLE_HYBRIDCLR
            ChangeState<StateLoadAssembly>();
#else
            ChangeState<StateStartGame>();
#endif
        }

        public override void Exit()
        {
        }
    }
}