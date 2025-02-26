#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ProxFramework.RedDot.Editor
{
    [CustomEditor(typeof(RedDotConfig))]
    public class RedDotConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Check Nodes"))
            {
                ((RedDotConfig)target).CheckForUselessNodes();
                ((RedDotConfig)target).CheckForCycles();
            }
        }
    }
}
#endif