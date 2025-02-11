using System;

namespace ProxFramework.Runtime.Settings
{
    [Serializable]
    public class HCLRSettings
    {
        public bool Enable
        {
            get
            {
#if ENABLE_HCLR
                return true;
#endif
                return false;
            }
        }

        public string[] hotUpdateAssemblies;
        public string[] aotMetaAssemblies;
        public string logicMainDllName;
        public string assemblyBytesAssetDir;
        public string assemblyBytesAssetExtension;
    }
}