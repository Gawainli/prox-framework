using System;
using System.Collections.Generic;
using System.Text;
using ProxFramework.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProxFramework.Utils
{
    //<color=#FF0000>红色文字</color>
    // <size=20>字体大小</size>
    // <b>加粗</b>
    // <i>斜体</i>
    // <u>下划线</u>
    // <s>删除线</s>
    // <mark=#FF0000>背景色</mark>
    // <link=click>点击事件</link>
    // <sprite=0>插入精灵
    // {key} 占位符,后续需要set value
    [RequireComponent(typeof(TMP_Text))]
    public class TMPRichText : MonoBehaviour, IPointerClickHandler
    {
        [Serializable]
        public class Placeholder
        {
            public string key;
            public string value;
        }

        public List<Placeholder> placeholders = new();
        public event Action<string> OnLinkClicked;

        private TMP_Text _tmpText;
        private string _originalText;
        private string _cachedText;
        private StringBuilder _sb = new();

        private void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            _originalText = _tmpText.text;
            _cachedText = _originalText;

            var localizedText = GetComponent<LocalizedText>();
            if (localizedText != null)
            {
                localizedText.OnLocaledTextApplied += OnLocaledTextApplied;
            }
        }

        private void Start()
        {
            ForceUpdateText();
        }

        public void SetValue(string key, string value)
        {
            var exists = false;
            foreach (var p in placeholders)
            {
                if (p.key != key) continue;
                p.value = value;
                exists = true;
                break;
            }

            if (!exists)
            {
                placeholders.Add(new Placeholder { key = key, value = value });
            }

            ForceUpdateText();
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

        private void OnLocaledTextApplied()
        {
            SetOriginalText(_tmpText.text, true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex =
                TMP_TextUtilities.FindIntersectingLink(_tmpText, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1) return;
            var linkInfo = _tmpText.textInfo.linkInfo[linkIndex];
            PLogger.Info(linkInfo.GetLinkID());
            OnLinkClicked?.Invoke(linkInfo.GetLinkID());
        }
    }
}