using System;
using UnityEngine;

namespace ProxFramework.Runtime.Settings
{
    public static class SettingsUtil
    {
        private const string SettingsPath = "FrameworkSettings";

#if UNITY_EDITOR
        private static EditorDevSettings _editorDevSettings;

        public static EditorDevSettings EditorDevSettings
        {
            get
            {
                if (_frameworkSettings == null)
                {
                    var assetType = typeof(FrameworkSettings);
                    string[] globalAssetPaths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
                    if (globalAssetPaths.Length == 0)
                    {
                        _editorDevSettings = ScriptableObject.CreateInstance<EditorDevSettings>();
                        UnityEditor.AssetDatabase.CreateAsset(_editorDevSettings, "Assets/EditorDevSettings.asset");
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                        Logger.Warning(
                            $"No Editor Dev Settings found in project. Created new one at Assets/EditorDevSettings.asset");
                    }

                    if (globalAssetPaths.Length > 1)
                    {
                        Logger.Warning(
                            $"More than one Editor Dev Settings found in project. We use first one. Path:{globalAssetPaths[0]}");
                    }

                    _editorDevSettings =
                        UnityEditor.AssetDatabase.LoadAssetAtPath<EditorDevSettings>(globalAssetPaths[0]);
                }

                return _editorDevSettings;
            }
        }

#endif

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