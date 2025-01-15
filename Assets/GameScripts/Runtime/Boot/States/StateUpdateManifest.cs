using ProxFramework.Asset;
using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StateUpdateManifest : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            // var succeed = await AssetModule.UpdateManifest();
            // if (succeed)
            // {
                // ChangeState<StateCreateDownloader>();
            // }
        }

        public override void Exit()
        {
        }
    }
}