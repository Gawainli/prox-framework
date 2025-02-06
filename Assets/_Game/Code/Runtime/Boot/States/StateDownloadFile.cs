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

        private int _perPackageDownloadCount;
        private long _pertPackageDownloadBytes;

        public override void Init()
        {
        }

        public override async void Enter()
        {
            var downloaderOpList =
                fsm.Blackboard.GetObjectValue<List<ResourceDownloaderOperation>>("totalDownloaderOp");
            foreach (var op in downloaderOpList)
            {
                _totalDownloadCount += op.TotalDownloadCount;
                _totalDownloadBytes += op.TotalDownloadBytes;
            }

            var downloadResult = await BeginDownload(downloaderOpList);
            if (downloadResult)
            {
#if UNITY_EDITOR
                await UniTask.WaitForSeconds(3f);
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
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
            _currentDownloadedCount += _perPackageDownloadCount;
            _currentDownloadedBytes += _pertPackageDownloadBytes;
            _perPackageDownloadCount = 0;
            _pertPackageDownloadBytes = 0;

            PLogger.Info(
                $"download package {data.PackageName} complete: {_currentDownloadedCount}/{_totalDownloadCount}, {_currentDownloadedBytes}/{_totalDownloadBytes}");
            if (_currentDownloadedCount == _totalDownloadCount)
            {
                PLogger.Info("all download complete");
            }
        }

        public void OnDownloadErrorCallback(DownloadErrorData errorData)
        {
            PLogger.Error($"download error: {errorData.PackageName} {errorData.FileName} {errorData.ErrorInfo}");
        }

        public void OnDownloadProgress(DownloadUpdateData updateData)
        {
            _perPackageDownloadCount = updateData.TotalDownloadCount;
            _pertPackageDownloadBytes = updateData.TotalDownloadBytes;
            PLogger.Info(
                $" download progress: {_currentDownloadedCount + _perPackageDownloadCount}/{_totalDownloadCount}, {_currentDownloadedBytes + _pertPackageDownloadBytes}/{_totalDownloadBytes}");


#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar("下载资源", $"下载资源中... {updateData.PackageName}",
                (float)(_currentDownloadedBytes + _pertPackageDownloadBytes) / _totalDownloadBytes);
#endif
        }
    }
}