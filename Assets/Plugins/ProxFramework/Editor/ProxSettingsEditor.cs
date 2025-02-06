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
        private static void ShowWindow()
        {
            var window = GetWindow<ProxSettingsEditor>(true);
            window.titleContent = new GUIContent("Prox Settings");
            window.Show();
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

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }
    }
}