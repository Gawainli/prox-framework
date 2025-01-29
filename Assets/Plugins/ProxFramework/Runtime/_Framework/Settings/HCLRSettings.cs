using System;

namespace ProxFramework.Runtime.Settings
{
    [Serializable]
    public class HCLRSettings
    {
        public string[] hotUpdateAssemblies;
        public string[] aotMetaAssemblies;
        public string logicMainDllName;
        public string assemblyBytesAssetDir;
        public string assemblyBytesAssetExtension;
    }
}