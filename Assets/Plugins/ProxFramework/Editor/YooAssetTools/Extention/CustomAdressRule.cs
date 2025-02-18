using System.IO;
using YooAsset.Editor;

namespace ProxFramework.Editor
{
    [DisplayName("定位地址: 文件名.智能尾缀")]
    public class AddressByFileNameAndExt : IAddressRule
    {
        public string GetAssetAddress(AddressRuleData data)
        {
            var ext = Path.GetExtension(data.AssetPath);
            if (ext == ".asset")
            {
                var a = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(data.AssetPath);
                if (a == null) return ".errortype";
                var type = a.GetType();
                var dt = Path.GetFileNameWithoutExtension(data.AssetPath);
                return dt + $".{type.Name.ToLowerInvariant()}";
            }

            return Path.GetFileName(data.AssetPath);
        }
    }
    
    [DisplayName("定位地址: 从CollectPath开始包含子目录")]
    public class AddressBySubCollectPath : IAddressRule
    {
        public string GetAssetAddress(AddressRuleData data)
        {
            var dir = Path.GetDirectoryName(data.AssetPath)
                .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            dir = dir.Replace(data.CollectPath + "/", "");
            var fileName = Path.GetFileNameWithoutExtension(data.AssetPath);
            var addressName = $"{dir}/{fileName}";
            return addressName.Replace("/", "_");
        }
    }
}