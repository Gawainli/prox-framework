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
            var downloaderOpList = new List<ResourceDownloaderOperation>();
            foreach (var pkg in AssetModule.GetAllPackages())
            {
                var op = AssetModule.CreateResourceDownloader(pkg.PackageName);
                if (op.TotalDownloadCount > 0)
                {
                    downloaderOpList.Add(op);
                }
            }

            if (downloaderOpList.Count == 0)
            {
                ChangeState<StatePatchDone>();
            }
            else
            {
                var downloadOp = downloaderOpList[0];
                for (int i = 1; i < downloaderOpList.Count; i++)
                {
                    downloadOp.Combine(downloaderOpList[i]);
                }
                fsm.SetBlackboardValue("downloaderOp",downloadOp);
                ChangeState<StateDownloadFile>();
            }
        }

        public override void Exit()
        {
        }
    }
}