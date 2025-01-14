using System;
using UnityEngine;

namespace ProxFramework.Runtime.Settings
{
    public static class SettingsUtil
    {
        private const string SettingsPath = "FrameworkSettings";
        private static FrameworkSettings _frameworkSettings;

        public static FrameworkSettings GlobalSettings
        {
            get
            {
                if (_frameworkSettings == null)
                {
                    _frameworkSettings = LoadSettingsFromResources();
                }

                return _frameworkSettings;
            }
        }

        private static FrameworkSettings LoadSettingsFromResources()
        {
#if UNITY_EDITOR
            var assetType = typeof(FrameworkSettings);
            string[] globalAssetPaths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
            if (globalAssetPaths.Length > 1)
            {
                foreach (var assetPath in globalAssetPaths)
                {
                    Logger.Error(
                        $"Could not had Multiple {assetType}. Repeated Path: {UnityEditor.AssetDatabase.GUIDToAssetPath(assetPath)}");
                }

                throw new Exception($"Could not had Multiple {assetType}");
            }
#endif
            var settings = Resources.Load<FrameworkSettings>(SettingsPath);
            if (settings != null) return settings;
            Debug.LogError($"Failed to load {typeof(FrameworkSettings)} from Resources at path: {SettingsPath}");
            throw new Exception(
                $"Failed to load {typeof(FrameworkSettings)} from Resources at path: {SettingsPath}");

        }
    }
}