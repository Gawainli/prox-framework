using System.Linq;
using UnityEngine;
using YooAsset;

namespace ProxFramework.Runtime.Settings
{
    [CreateAssetMenu(fileName = "Editor Settings", menuName = "Prox/Editor Settings")]
    public class EditorDevSettings : ScriptableObject
    {
        public EPlayMode playMode;
        public string internalHostServer;
        public bool splitArtRes;
        public string[] splitArtResPackageNames;

        public EPlayMode GetPackageDevPlayMode(string packageName)
        {
            if (playMode == EPlayMode.EditorSimulateMode && splitArtRes &&
                splitArtResPackageNames.Contains(packageName))
            {
                return EPlayMode.HostPlayMode;
            }

            return playMode;
        }
    }
}