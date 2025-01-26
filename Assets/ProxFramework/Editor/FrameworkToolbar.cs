#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using YooAsset.Editor;

namespace ProxFramework.Editor
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                fixedWidth = 0,
                fixedHeight = 0,
                padding = new RectOffset(10, 10, 0, 0),
            };
        }
    }

    [InitializeOnLoad]
    public static class FrameworkToolbar
    {
        static FrameworkToolbar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }
        
        private static void RunGame()
        {
            if (EditorTools.HasDirtyScenes())
            {
                EditorUtility.DisplayDialog("Warning",
                    "检测到未保存scene！运行游戏前保存当前scene.", "OK");
            }
            else
            {
                //切到0场景
                var path = EditorBuildSettings.scenes[0].path;
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                EditorApplication.isPlaying = true;
            }
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Run Game", "Start Scene 0"), ToolbarStyles.commandButtonStyle))
            {
                RunGame();
            }
        }
    }
}
#endif