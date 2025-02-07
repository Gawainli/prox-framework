using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace ProxFramework.Asset
{
    public static partial class AssetModule
    {
        public static string GetPackageVersion(string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(defaultPkgName)
                : YooAssets.GetPackage(packageName);
            return package == null ? string.Empty : package.GetPackageVersion();
        }

        public static async UniTask<RequestPackageVersionOperation> UpdatePackageVersionAsync(
            bool appendTimeTicks = false, int timeout = 60,
            string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(defaultPkgName)
                : YooAssets.GetPackage(packageName);
            var op = package.RequestPackageVersionAsync(appendTimeTicks, timeout);
            await op.ToUniTask(cancellationToken: CtsToken);
            return op;
        }

        public static async UniTask<UpdatePackageManifestOperation> UpdatePackageManifestAsync(string packageVersion,
            int timeout = 60, string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(defaultPkgName)
                : YooAssets.GetPackage(packageName);
            var op = package.UpdatePackageManifestAsync(packageVersion, timeout);
            await op.ToUniTask(cancellationToken: CtsToken);
            return op;
        }

        public static ResourceDownloaderOperation CreateResourceDownloader(string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(defaultPkgName)
                : YooAssets.GetPackage(packageName);
            var downloadOp = package.CreateResourceDownloader(_downloadingMaxNum, _failedTryAgain);
            return downloadOp;
        }

        public static ResourceDownloaderOperation CreateResourceDownloaderForAll()
        {
            var package = YooAssets.GetPackage(defaultPkgName);
            var downloadOp = package.CreateResourceDownloader(_downloadingMaxNum, _failedTryAgain);
            foreach (var pkg in GetAllPackages())
            {
                if (package == pkg) continue;
                var op = pkg.CreateResourceDownloader(_downloadingMaxNum, _failedTryAgain);
                downloadOp.Combine(op);
            }

            return downloadOp;
        }

        public static PatchAsyncOperation CreatePatchAsyncOperation()
        {
            var downloaderOpList = new List<ResourceDownloaderOperation>();

            foreach (var pkg in GetAllPackages())
            {
                var op = pkg.CreateResourceDownloader(_downloadingMaxNum, _failedTryAgain);
                if (op.TotalDownloadCount == 0)
                {
                    continue;
                }

                downloaderOpList.Add(op);
            }

            return new PatchAsyncOperation(downloaderOpList);
        }

        public static async UniTask<ClearCacheFilesOperation> ClearUnusedCacheFilesAsync(string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(defaultPkgName)
                : YooAssets.GetPackage(packageName);
            var op = package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
            await op.ToUniTask(cancellationToken: CtsToken);
            return op;
        }
    }
}