using System;
using ProxFramework.Asset;
using UnityEditor;

namespace ProxFramework.Localization
{
    public abstract class LocalizedAsset<T> : LocalizedBehaviour where T : UnityEngine.Object
    {
        public override async void ApplyLocalization()
        {
            try
            {
                if (string.IsNullOrEmpty(l10NKey))
                {
                    PLogger.Warning($"Asset path is empty, please set it first. {gameObject.name}");
                    return;
                }

                var localizedPath = LocalizationModule.GetLocalizeAsstPath(l10NKey);
                // 如果本地化路径为空或者不合法，则使用原始路径
                if (string.IsNullOrEmpty(localizedPath) || !AssetModule.CheckLocationValid(localizedPath))
                {
                    PLogger.Warning($"Localized asset is null. {gameObject.name}. asset path:{localizedPath}");
                    localizedPath = l10NKey;
                }

                var asset = await AssetModule.LoadAssetAsync<T>(localizedPath);
                if (asset != null)
                {
                    ApplyAsset(asset);
                }
            }
            catch (Exception e)
            {
                PLogger.Error($"LocalizedAsset.LoadLocalizedAsset: {e}");
            }
        }

        protected abstract void ApplyAsset(T asset);
    }
}