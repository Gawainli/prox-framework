using ProxFramework.Asset;
using ProxFramework.StateMachine;

namespace ProxFramework.Base
{
    public class StateCreateDownloader : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            // if (AssetModule.CreateDownloader())
            // {
                // ChangeState<StateDownloadFile>();
            // }
            // else
            // {
                // ChangeState<StateClearCache>();
            // }
        }

        public override void Exit()
        {
        }
    }
}