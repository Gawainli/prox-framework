using ProxFramework.UI;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace ProxFramework.Runtime.Settings
{
    // [CreateAssetMenu(fileName = "FrameworkSettings", menuName = "Prox/Framework Settings")]
    public class FrameworkSettings : ScriptableObject
    {
        public int targetFrameRate = 60;

        [SerializeField] private EPlayMode runtimePlayMode;
        [SerializeField] private EPlayMode editorPlayMode;

        public EPlayMode PlayMode
        {
            get
            {
#if UNITY_EDITOR
                return editorPlayMode;
#else
                return runtimePlayMode;
#endif
            }
        }

        public AssetSettings assetSettings;
        public HCLRSettings hclrSettings;
        public UISettings uiSettings;
        public DataTableSettings dataTableSettings;
    }
}