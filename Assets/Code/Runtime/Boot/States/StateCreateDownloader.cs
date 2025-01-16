using System.Collections.Generic;
using ProxFramework.Asset;
using ProxFramework.StateMachine;
using YooAsset;

namespace GameName.Runtime
{
    public class StateCreateDownloader : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            var downloaderOp = AssetModule.CreateResourceDownloaderForAll();
            if (downloaderOp.TotalDownloadCount == 0)
            {
                ChangeState<StatePatchDone>();
            }
            else
            {
                fsm.SetBlackboardValue("downloaderOp", downloaderOp);
                ChangeState<StateDownloadFile>();
            }
        }

        public override void Exit()
        {
        }
    }
}