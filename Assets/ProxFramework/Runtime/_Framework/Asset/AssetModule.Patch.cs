using Cysharp.Threading.Tasks;
using YooAsset;

namespace ProxFramework.Asset
{
    public partial class AssetModule
    {
        public string GetPackageVersion(string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(DefaultPkgName)
                : YooAssets.GetPackage(packageName);
            return package == null ? string.Empty : package.GetPackageVersion();
        }

        public RequestPackageVersionOperation UpdatePackageVersionAsync(bool appendTimeTicks = false, int timeout = 60,
            string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(DefaultPkgName)
                : YooAssets.GetPackage(packageName);
            return package.RequestPackageVersionAsync(appendTimeTicks, timeout);
        }

        public UpdatePackageManifestOperation UpdatePackageManifestAsync(string packageVersion,
            int timeout = 60, string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(DefaultPkgName)
                : YooAssets.GetPackage(packageName);
            return package.UpdatePackageManifestAsync(packageVersion, timeout);
        }

        public ResourceDownloaderOperation CreateResourceDownloader(string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(DefaultPkgName)
                : YooAssets.GetPackage(packageName);
            var downloadOp = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            return downloadOp;
        }
        
        public ClearCacheFilesOperation ClearUnusedCacheFilesAsync(string packageName = "")
        {
            var package = string.IsNullOrEmpty(packageName)
                ? YooAssets.GetPackage(DefaultPkgName)
                : YooAssets.GetPackage(packageName);
            return package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
        }
    }
}