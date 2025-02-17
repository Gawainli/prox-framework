using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ProxFramework.Localization
{
    public class LocalizedImage : LocalizedAsset<Sprite>
    {
        private Image _image;

        protected override void UpdateAssetPath()
        {
            var image = GetComponent<Image>();
            if (image == null)
            {
                PLogger.Warning("LocalizedImage: Image component not found");
                assetPath = "";
                return;
            }

            if (image.sprite == null)
            {
                PLogger.Warning("LocalizedImage: Image sprite is null");
                assetPath = "";
                return;
            }

            assetPath = AssetDatabase.GetAssetPath(image.sprite);
        }

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