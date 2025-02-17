namespace ProxFramework.Localization
{
    public interface IL10NTable
    {
        string GetValue(string languageCode, string key);
        bool TryGetValue(string languageCode, string key, out string value);
    }
}