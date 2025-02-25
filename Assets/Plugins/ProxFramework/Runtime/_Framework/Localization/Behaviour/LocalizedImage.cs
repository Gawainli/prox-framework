using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ProxFramework.Localization
{
    public class LocalizedImage : LocalizedAsset<Sprite>
    {
        private Image _image;

#if UNITY_EDITOR
        public override void AutoSetL10NKey()
        {
            var asset = GetComponent<Image>();
            if (asset == null)
            {
                PLogger.Warning($"Localized {typeof(Image)} is null. {gameObject.name}");
                l10NKey = string.Empty;
                return;
            }

            var path = AssetDatabase.GetAssetPath(asset.sprite);
            l10NKey = path;
        }
#endif

        protected override void ApplyAsset(Sprite asset)
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            _image.sprite = asset;
        }
    }
}