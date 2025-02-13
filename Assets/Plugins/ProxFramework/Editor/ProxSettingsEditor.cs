#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ProxFramework.Editor
{
    public class ProxSettingsEditor : EditorWindow
    {
        private const string SettingsAssetPath = "Assets/Resources/FrameworkSettings.asset";
        private Runtime.Settings.FrameworkSettings _settings;
        private UnityEditor.Editor _editor;

        [MenuItem("Prox/Settings")]
        private static void OpenWindow()
        {
            var window = GetWindow<ProxSettingsEditor>("Prox Settings", true);
            window.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            // Load settings
            _settings = AssetDatabase.LoadAssetAtPath<Runtime.Settings.FrameworkSettings>(SettingsAssetPath);
            if (_settings == null)
            {
                _settings = ScriptableObject.CreateInstance<Runtime.Settings.FrameworkSettings>();
                AssetDatabase.CreateAsset(_settings, SettingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            _editor = UnityEditor.Editor.CreateEditor(_settings);
        }

        private void OnGUI()
        {
            if (_settings == null || _editor == null)
            {
                return;
            }

            EditorGUILayout.LabelField($"Settings Path: {SettingsAssetPath}");
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            _editor.OnInspectorGUI();
#if ENABLE_HCLR
            if (GUILayout.Button("Refresh HCLR Settings"))
            {
                RefreshHclrSettings();
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }

#endif
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }

        private void RefreshHclrSettings()
        {
            if (_settings == null || !_settings.hclrSettings.Enable)
            {
                return;
            }

            // Load HCLR settings
            var hotUpdateAssemblyNames = HybridCLR.Editor.SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;
            foreach (var dllName in hotUpdateAssemblyNames)
            {
                PLogger.Info($"HotUpdateAssemblyNamesExcludePreserved: {dllName}");
            }

            var aotMetaAssemblyNames = HybridCLR.Editor.SettingsUtil.AOTAssemblyNames;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = assembly.GetType("AOTGenericReferences");
                if (t == null) continue;

                var patchedAOTList = (List<string>)t.GetField("PatchedAOTAssemblyList").GetValue(null);
                foreach (var metaDllName in patchedAOTList)
                {
                    aotMetaAssemblyNames.Add(Path.GetFileNameWithoutExtension(metaDllName));
                }
            }

            foreach (var metaDllName in aotMetaAssemblyNames)
            {
                PLogger.Info($"AOTAssemblyNames: {metaDllName}");
            }

            foreach (var hotUpdateName in _settings.hclrSettings.hotUpdateAssemblies)
            {
                if (!hotUpdateAssemblyNames.Contains(hotUpdateName))
                {
                    hotUpdateAssemblyNames.Add(hotUpdateName);
                }
            }

            foreach (var aotDllName in _settings.hclrSettings.aotMetaAssemblies)
            {
                if (!aotMetaAssemblyNames.Contains(aotDllName))
                {
                    aotMetaAssemblyNames.Add(aotDllName);
                }
            }
            
            _settings.hclrSettings.hotUpdateAssemblies = hotUpdateAssemblyNames.ToArray();
            _settings.hclrSettings.aotMetaAssemblies = aotMetaAssemblyNames.ToArray();
        }
    }
}
#endif