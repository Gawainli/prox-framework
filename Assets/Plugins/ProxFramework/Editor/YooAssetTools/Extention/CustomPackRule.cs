﻿#if UNITY_EDITOR

using System;
using System.IO;
using YooAsset.Editor;

namespace ProxFramework.Editor
{
    [DisplayName("打包特效纹理（自定义）")]
    public class PackEffectTexture : IPackRule
    {
        private const string PackDirectory = "Assets/Effect/Textures/";

        PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
        {
            string assetPath = data.AssetPath;
            if (assetPath.StartsWith(PackDirectory) == false)
                throw new Exception($"Only support folder : {PackDirectory}");

            string assetName = Path.GetFileName(assetPath).ToLower();
            string firstChar = assetName.Substring(0, 1);
            string bundleName = $"{PackDirectory}effect_texture_{firstChar}";
            var packRuleResult = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return packRuleResult;
        }
    }

    [DisplayName("打包视频（自定义）")]
    public class PackVideo : IPackRule
    {
        public PackRuleResult GetPackRuleResult(PackRuleData data)
        {
            string bundleName = RemoveExtension(data.AssetPath);
            string fileExtension = Path.GetExtension(data.AssetPath);
            fileExtension = fileExtension.Remove(0, 1);
            PackRuleResult result = new PackRuleResult(bundleName, fileExtension);
            return result;
        }

        private string RemoveExtension(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int index = str.LastIndexOf(".");
            if (index == -1)
                return str;
            else
                return str.Remove(index); //"assets/config/test.unity3d" --> "assets/config/test"
        }
    }
}
#endif