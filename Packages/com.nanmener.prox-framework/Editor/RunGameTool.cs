#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using YooAsset.Editor;

namespace ProxFramework.Editor
{
    public class RunGameTool
    {
        [MenuItem("Prox/Run Game")]
        public static void RunGame()
        {
            if (EditorTools.HasDirtyScenes())
            {
                EditorUtility.DisplayDialog("Warning",
                    "检测到未保存scene！运行游戏前保存当前scene.", "OK");
            }
            else
            {
                //切到0场景
                EditorSceneManager.OpenScene("Assets/_Game/_Launcher.unity", OpenSceneMode.Single);
                EditorApplication.isPlaying = true;
            }
        }
    }
}
#endif