using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace ProxFramework.Runtime.Settings
{
    [CreateAssetMenu(fileName = "Editor Settings", menuName = "Prox/Editor Settings")]
    public class EditorDevSettings : ScriptableObject
    {
        public EPlayMode assetPlayMode;
        public string internalHostServer;
        public bool splitArtRes;
        public string[] splitArtResPackageNames;

        public EPlayMode GetPackageDevPlayMode(string packageName)
        {
            if (assetPlayMode == EPlayMode.EditorSimulateMode && splitArtRes &&
                splitArtResPackageNames.Contains(packageName))
            {
                return EPlayMode.HostPlayMode;
            }

            return assetPlayMode;
        }
    }
}