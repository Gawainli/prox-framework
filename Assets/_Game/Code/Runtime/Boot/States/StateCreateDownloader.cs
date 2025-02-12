using ProxFramework.Asset;
using ProxFramework.StateMachine;

namespace Prox.GameName.Runtime
{
    public class StateCreateDownloader : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            var patchOp = AssetModule.CreatePatchAsyncOperation();
            if (patchOp.totalDownloadCount == 0)
            {
                ChangeState<StatePatchDone>();
            }
            else
            {
                //TODO:can check space here
                fsm.Blackboard.SetObjectValue("patchOp", patchOp);
                ChangeState<StateDownloadFile>();
            }
        }

        public override void Exit()
        {
        }
    }
}