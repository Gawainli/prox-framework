using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.StateMachine;
using YooAsset;

namespace GameName.Runtime
{
    public class StateDownloadFile : State
    {
        private int _totalDownloadCount;
        private long _totalDownloadBytes;


        public override void Init()
        {
        }

        public override async void Enter()
        {
            var downloaderOp = fsm.GetBlackboardValue<ResourceDownloaderOperation>("downloaderOp");
            _totalDownloadCount = downloaderOp.TotalDownloadCount;
            _totalDownloadBytes = downloaderOp.TotalDownloadBytes;
            downloaderOp.DownloadErrorCallback = OnDownloadErrorCallback;
            downloaderOp.DownloadUpdateCallback = OnDownloadProgress;
            downloaderOp.DownloadFinishCallback = OnDownloadFinishCallback;
            downloaderOp.DownloadFileBeginCallback = OnDownloadFileBeginCallback;
            downloaderOp.BeginDownload();
            await downloaderOp.ToUniTask();
            
            if (downloaderOp.Status == EOperationStatus.Succeed)
            {
                ChangeState<StatePatchDone>();
            }
        }

        public override void Exit()
        {
        }

        private void OnDownloadFileBeginCallback(DownloadFileData data)
        {
            PLogger.Info($"download file:{data.FileName} size:{data.FileSize} in package:{data.PackageName}");
        }

        private void OnDownloadFinishCallback(DownloaderFinishData data)
        {
            PLogger.Info(
                $"download package {data.PackageName} complete.");
        }

        public void OnDownloadErrorCallback(DownloadErrorData errorData)
        {
            PLogger.Error($"download error: {errorData.PackageName} {errorData.FileName} {errorData.ErrorInfo}");
        }

        public void OnDownloadProgress(DownloadUpdateData updateData)
        {
            PLogger.Info(
                $" download progress: {updateData.TotalDownloadCount}/{_totalDownloadCount}, {updateData.TotalDownloadBytes}/{_totalDownloadBytes}");
        }
    }
}