using Cysharp.Threading.Tasks;
using ProxFramework.Asset;
using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StateClearCache : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            // await AssetModule.assetPkg.ClearAllCacheFilesAsync().ToUniTask();
            ChangeState<StatePatchDone>();
        }

        public override void Exit()
        {
        }
    }
}