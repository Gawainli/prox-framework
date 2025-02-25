namespace ProxFramework.Localization
{
    public interface II18NTable
    {
        string GetValue(string languageCode, string key);
        bool TryGetValue(string languageCode, string key, out string value);
    }
}