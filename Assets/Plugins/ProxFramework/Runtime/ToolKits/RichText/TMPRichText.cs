using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace ProxFramework.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMPRichText : MonoBehaviour
    {
        [System.Serializable]
        public class Placeholder
        {
            public string key;
            public string value;
        }

        public List<Placeholder> placeholders = new();

        private TMP_Text _tmpText;
        private string _originalText;
        private string _cachedText;
        private StringBuilder _sb = new StringBuilder();

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            _originalText = _tmpText.text;
            _cachedText = _originalText;
        }

        public void SetValue(string key, string value)
        {
            foreach (var item in placeholders)
            {
                if (item.key == key)
                {
                    item.value = value;
                    ForceUpdateText();
                    return;
                }
            }

            PLogger.Warning($"TMPRichText: Placeholder with key [{key}] not found");
        }

        private void ForceUpdateText()
        {
            _sb.Clear();
            _sb.Append(_originalText);
            foreach (var item in placeholders)
            {
                _sb.Replace($"{{{item.key}}}", item.value);
            }

            // 仅在文本确实变化时更新Mesh
            string newText = _sb.ToString();
            if (newText == _cachedText) return;
            _cachedText = newText;
            _tmpText.text = newText;
            _tmpText.ForceMeshUpdate(); // 强制TMP重新渲染
        }

        private void OnDestroy()
        {
            _sb?.Clear();
            _sb = null;
            _tmpText = null; 
        }

        public void SetOriginalText(string originalText, bool forceUpdate)
        {
            _tmpText = GetComponent<TMP_Text>();
            _tmpText.text = originalText;
            _originalText = originalText;
            _cachedText = string.Empty;
            if (forceUpdate)
            {
                ForceUpdateText();
            }
        }
    }
}