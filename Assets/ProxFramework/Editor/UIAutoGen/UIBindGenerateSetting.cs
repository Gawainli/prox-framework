#if UNITY_EDITOR
using System;
using UnityEngine;

namespace ProxFramework.Editor
{
    [Serializable]
    [CreateAssetMenu(fileName = "UIBindGenerateSetting", menuName = "Prox/UIBindGenerateSetting")]
    public class UIBindGenerateSetting : ScriptableObject
    {
        [Header("Config")]
        [Tooltip("Path to the UI Window Code Template")]
        public string uiWindowCodeTemplatePath;

        [Header("Config")]
        [Tooltip("Path to the UI Window Gen Template")]
        public string uiWindowGenTemplatePath;

        [Header("Config")]
        [Tooltip("Path to the UI Code Generate Directory")]
        public string uiCodeGeneratePath;

        [Header("Config")]
        [Tooltip("Namespace for the generated UI Code")]
        public string uiCodeNamespace;
    }
}
#endif