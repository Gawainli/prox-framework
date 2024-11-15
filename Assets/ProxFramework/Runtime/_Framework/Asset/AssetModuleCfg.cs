using UnityEngine;
using YooAsset;

namespace ProxFramework.Asset
{
    public class AssetModuleCfg
    {
        public string assetPkgName;
        public string rawPkgName;
        public EPlayMode ePlayMode;
        public string hostServerIP;
        public string appVersion;
        public string DefaultHostServer => GetHostServerURL();

        private string GetHostServerURL()
        {
            if (string.IsNullOrEmpty(this.hostServerIP) || string.IsNullOrEmpty(this.appVersion))
            {
                hostServerIP = "http://127.0.0.1";
                appVersion = "v1.0";
            }
#if UNITY_EDITOR
           
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/CDN/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/CDN/IPhone/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                // return $"{hostServerIP}/CDN/WebGL/{appVersion}";
			    return $"{hostServerIP}/StreamingAssets/yoo/DefaultPackage";
            else
                return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
		if (UnityEngine.Device.Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android/{appVersion}";
		else if (UnityEngine.Device.Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone/{appVersion}";
		else if (UnityEngine.Device.Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/StreamingAssets/yoo/DefaultPackage";
		else
			return $"{hostServerIP}/CDN/PC/{appVersion}";
        return "";
#endif
        }
    }
}