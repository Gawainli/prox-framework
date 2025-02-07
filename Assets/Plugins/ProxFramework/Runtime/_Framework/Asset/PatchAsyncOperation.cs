using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace ProxFramework.Asset
{
    public class PatchAsyncOperation : CustomYieldInstruction
    {
        private int _downloadFinishCount;
        private int _currentOpDownloadCount;
        public int CurrentDownloadCount => _downloadFinishCount + _currentOpDownloadCount;

        private long _downloadFinishBytes;
        private long _currentOpDownloadBytes;
        public long CurrentDownloadBytes => _downloadFinishBytes + _currentOpDownloadBytes;

        public float Progress => (float)CurrentDownloadBytes / totalDownloadBytes;

        public int totalDownloadCount;
        public long totalDownloadBytes;
        public override bool keepWaiting => !_isDone;

        public bool succeed;
        public string errorInfo;

        private bool _isDone;
        private ResourceDownloaderOperation _currentOp;
        private List<ResourceDownloaderOperation> _finishedOps = new();
        private Queue<ResourceDownloaderOperation> _downloaderOperations = new();

        public PatchAsyncOperation(List<ResourceDownloaderOperation> downloaderOperations)
        {
            totalDownloadCount = 0;
            totalDownloadBytes = 0;
            foreach (var op in downloaderOperations)
            {
                totalDownloadCount += op.TotalDownloadCount;
                totalDownloadBytes += op.TotalDownloadBytes;
                _downloaderOperations.Enqueue(op);
            }
        }

        public void BeginDownload()
        {
            if (_downloaderOperations.Count == 0)
            {
                _currentOp = null;
                succeed = true;
                _isDone = true;
                return;
            }

            _currentOp = _downloaderOperations.Dequeue();
            _currentOp.DownloadErrorCallback = OnDownloadErrorCallback;
            _currentOp.DownloadUpdateCallback = OnDownloadProgress;
            _currentOp.DownloadFinishCallback = OnDownloadFinishCallback;
            _currentOp.DownloadFileBeginCallback = OnDownloadFileBeginCallback;
            _currentOp.BeginDownload();
        }

        public void PauseDownload()
        {
            _currentOp?.PauseDownload();
        }

        public void ResumeDownload()
        {
            _currentOp?.ResumeDownload();
        }

        public void CancelDownload()
        {
            _currentOp?.CancelDownload();
            _isDone = true;
        }

        private void OnDownloadFileBeginCallback(DownloadFileData data)
        {
        }

        private void OnDownloadFinishCallback(DownloaderFinishData data)
        {
            _currentOp.DownloadErrorCallback = null;
            _currentOp.DownloadUpdateCallback = null;
            _currentOp.DownloadFinishCallback = null;
            _currentOp.DownloadFileBeginCallback = null;

            _downloadFinishCount += _currentOp.TotalDownloadCount;
            _downloadFinishBytes += _currentOp.TotalDownloadBytes;
            _currentOpDownloadBytes = 0;
            _currentOpDownloadCount = 0;
            _finishedOps.Add(_currentOp);
            BeginDownload();
        }

        private void OnDownloadProgress(DownloadUpdateData data)
        {
            _currentOpDownloadBytes = data.CurrentDownloadBytes;
            _currentOpDownloadCount = data.CurrentDownloadCount;
        }

        private void OnDownloadErrorCallback(DownloadErrorData data)
        {
            succeed = false;
            errorInfo = data.ErrorInfo;
            _isDone = true;
        }
    }
}