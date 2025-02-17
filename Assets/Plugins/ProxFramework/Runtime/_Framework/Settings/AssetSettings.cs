using System;

namespace ProxFramework.Runtime.Settings
{
    public enum AssetEncryptType
    {
        None,
        Offset,
        Xor
    }

    [Serializable]
    public struct AssetPackageDefine
    {
        public string name;
        public bool isRaw;
        public AssetEncryptType encryptType;
        public string devHostUrl;
    }

    [Serializable]
    public class AssetSettings
    {
        public string scriptVersion;
        public string preloadTags;
        public string assetCdn;
        public int maxDownloadingNum;
        public int failedTryAgain;
        public AssetPackageDefine[] packages;
    }
}