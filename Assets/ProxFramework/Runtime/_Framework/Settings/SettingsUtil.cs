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
            string[] assetGuids = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
            if (assetGuids.Length > 1)
            {
                foreach (var guid in assetGuids)
                {
                    PLogger.Error(
                        $"Could not had Multiple {assetType}. Repeated Path: {UnityEditor.AssetDatabase.GUIDToAssetPath(guid)}");
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