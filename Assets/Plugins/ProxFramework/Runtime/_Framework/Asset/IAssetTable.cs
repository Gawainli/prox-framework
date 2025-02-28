namespace ProxFramework.Asset
{
    public interface IAssetTable
    {
        string GetValue(string key, string regionCode = null);
    }
}