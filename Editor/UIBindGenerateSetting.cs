#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProxFramework.Editor
{
    [Serializable]
    [CreateAssetMenu(fileName = "UIBindGenerateSetting", menuName = "Prox/UIBindGenerateSetting")]
    public class UIBindGenerateSetting : ScriptableObject
    {
        [BoxGroup("Config")] [Sirenix.OdinInspector.FilePath(Extensions = "cs", RequireExistingPath = true)]
        public string uiWindowCodeTemplatePath;

        [BoxGroup("Config")] [Sirenix.OdinInspector.FilePath(Extensions = "cs", RequireExistingPath = true)]
        public string uiWindowGenTemplatePath;

        [BoxGroup("Config")] [FolderPath(RequireExistingPath = true)]
        public string uiCodeGeneratePath;

        [BoxGroup("Config")] public string uiCodeNamespace;
    }
}
#endif