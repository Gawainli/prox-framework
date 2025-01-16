using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ProxFramework;
using ProxFramework.StateMachine;
using YooAsset;

namespace GameName.Runtime
{
    public class StateDownloadFile : State
    {
        private int _currentDownloadedCount;
        private int _totalDownloadCount;

        private long _currentDownloadedBytes;
        private long _totalDownloadBytes;

        private int _currentPackageDownloadCount;
        private long _currentPackageDownloadBytes;

        public override void Init()
        {
        }

        public override async void Enter()
        {
            var downloaderOpList = fsm.GetBlackboardValue<List<ResourceDownloaderOperation>>("totalDownloaderOp");
            foreach (var op in downloaderOpList)
            {
                _totalDownloadCount += op.TotalDownloadCount;
                _totalDownloadBytes += op.TotalDownloadBytes;
            }

            var downloadResult = await BeginDownload(downloaderOpList);
            if (downloadResult)
            {
                ChangeState<StatePatchDone>();
            }
        }

        public override void Exit()
        {
        }

        private async UniTask<bool> BeginDownload(List<ResourceDownloaderOperation> downloaderOpList)
        {
            foreach (var op in downloaderOpList)
            {
                op.DownloadErrorCallback = OnDownloadErrorCallback;
                op.DownloadUpdateCallback = OnDownloadProgress;
                op.DownloadFinishCallback = OnDownloadFinishCallback;
                op.DownloadFileBeginCallback = OnDownloadFileBeginCallback;
                op.BeginDownload();
                await op;

                if (op.Status != EOperationStatus.Succeed)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnDownloadFileBeginCallback(DownloadFileData data)
        {
            PLogger.Info($"download file:{data.FileName} size:{data.FileSize} in package:{data.PackageName}");
        }

        private void OnDownloadFinishCallback(DownloaderFinishData data)
        {
            _currentDownloadedCount += _currentPackageDownloadCount;
            _currentDownloadedBytes += _currentPackageDownloadBytes;
            _currentPackageDownloadCount = 0;
            _currentPackageDownloadBytes = 0;

            PLogger.Info(
                $"download package {data.PackageName} complete: {_currentDownloadedCount}/{_totalDownloadCount}, {_currentDownloadedBytes}/{_totalDownloadBytes}");
        }

        public void OnDownloadErrorCallback(DownloadErrorData errorData)
        {
            PLogger.Error($"download error: {errorData.PackageName} {errorData.FileName} {errorData.ErrorInfo}");
        }

        public void OnDownloadProgress(DownloadUpdateData updateData)
        {
            _currentPackageDownloadCount = updateData.TotalDownloadCount;
            _currentPackageDownloadBytes = updateData.TotalDownloadBytes;
            PLogger.Info(
                $" download progress: {_currentDownloadedCount + _currentPackageDownloadCount}/{_totalDownloadCount}, {_currentDownloadedBytes + _currentPackageDownloadBytes}/{_totalDownloadBytes}");
        }
    }
}