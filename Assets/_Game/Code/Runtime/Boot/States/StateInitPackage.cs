using ProxFramework;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using YooAsset;

namespace Prox.GameName.Runtime
{
    public class StateInitPackage : State
    {
        public override void Init()
        {
        }

        public override async void Enter()
        {
            var status = await AssetModule.InitPackages();
            if (status == EOperationStatus.Succeed)
            {
                ChangeState<StateUpdateVersion>();
            }
            else
            {
                PLogger.Error("StateInitPackage failed");
            }
        }

        public override void Exit()
        {
        }
    }
}