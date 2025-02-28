#if UNITY_EDITOR

using System;
using Cysharp.Threading.Tasks;
using GameName.Core;
using ProxFramework.Localization;
using ProxFramework.Runtime.Settings;
using UnityEditor;
using UnityEngine;

namespace ProxFramework.Editor.L10N
{
    public class I18NToolsEditor : EditorWindow
    {
        private static int selectedLangIndex = 0;

        [MenuItem("Prox/i18n")]
        public static async void ShowWindow()
        {
            await InitLocalization();
            GetWindow<I18NToolsEditor>("Localization Preview");
        }

        private void OnDestroy()
        {
            LocalizationModule.ChangeLanguage(LocalizationModule.DefaultLanguage);
            UpdateSceneLocalizedBehaviours();
            LocalizationModule.Shutdown();
        }

        private static async UniTask InitLocalization()
        {
            if (LocalizationModule.initialized)
            {
                LocalizationModule.Shutdown();
            }

            await TableManager.Initialize();
            await LocalizationModule.Initialize(new cfg.I18NTable());
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            DrawLanguageSelector();
            DrawSceneObjectPreview();

            EditorGUILayout.EndVertical();
        }

        void DrawLanguageSelector()
        {
            EditorGUILayout.BeginHorizontal();
            // previewLang = (SystemLanguage) EditorGUILayout.EnumPopup("Editor Language", previewLang);
            var options =
                LocalizationModule.SupportLanguages.ConvertAll(lang => lang.ToString());
            selectedLangIndex = EditorGUILayout.Popup("Preview Language", selectedLangIndex, options.ToArray());
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSceneObjectPreview()
        {
            EditorGUILayout.BeginHorizontal();
            var previewLang = LocalizationModule.SupportLanguages[selectedLangIndex];
            if (GUILayout.Button("Refresh"))
            {
                LocalizationModule.ChangeLanguage(previewLang);
                UpdateSceneLocalizedBehaviours();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateSceneLocalizedBehaviours()
        {
            var localizedBehaviours = FindObjectsOfType<LocalizedBehaviour>();
            foreach (var localizedBehaviour in localizedBehaviours)
            {
                localizedBehaviour.ApplyLocalization();
            }

            SceneView.RepaintAll();
        }
    }
}
#endif