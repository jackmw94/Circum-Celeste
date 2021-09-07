namespace Code.Level.Player
{
    public interface IPlayerPrefsProvider
    {
        int GetInt(string key, int defaultValue = 0);
        float GetFloat(string key, float defaultValue = 0.0f);
        string GetString(string key, string defaultValue = "");
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetString(string key, string value);
        void Save();
        void DeleteKey(string key);
        void DeleteAll();
    }
}