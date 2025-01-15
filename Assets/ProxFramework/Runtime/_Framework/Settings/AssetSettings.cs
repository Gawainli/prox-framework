using System;

namespace ProxFramework.Runtime.Settings
{
    [Serializable]
    public class AssetSettings
    {
        public string scriptVersion;
        public string preloadTags;
        public string assetCdn;
        public string defaultPackageName;
        public string defaultRawPackageName;
        public string[] packageNames;
        public string[] rawPackageNames;
    }
}