using ProxFramework.Asset;
using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StateUpdateVersion : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            var succeed = await AssetModule.GetStaticVersion();
            if (succeed)
            {
                ChangeState<StateUpdateManifest>();
            }
            
        }

        public override void Exit()
        {
        }
    }
}