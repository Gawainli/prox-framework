using TMPro;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace ProxFramework.Utils.Editor
{
    [CustomEditor(typeof(TMPRichText))]
    public class TMPRichTextEditor : UnityEditor.Editor
    {
        private string _originText;
        private bool _isPreviewing;

        private void OnDestroy()
        {
            var tmpRichText = target as TMPRichText;
            if (_isPreviewing)
            {
                tmpRichText?.SetOriginalText(_originText, false);
            }
            // tmpRichText?.Reset();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!_isPreviewing && GUILayout.Button("Preview"))
            {
                _isPreviewing = true;
                var tmpRichText = target as TMPRichText;
                _originText = tmpRichText?.GetComponent<TMP_Text>().text;
                tmpRichText?.SetOriginalText(_originText, true);
            }

            //button to stop preview with green color
            if (_isPreviewing)
            {
                var color = GUI.color;
                GUI.color = Color.green;
                if (GUILayout.Button("Previewing"))
                {
                    _isPreviewing = false;
                    var tmpRichText = target as TMPRichText;
                    tmpRichText?.SetOriginalText(_originText, false);
                    SceneView.RepaintAll();
                }

                GUI.color = color;
            }
        }
    }
}
#endif