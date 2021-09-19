namespace Code.Core
{
    public static class PlayerPrefsKeys
    {
#if PERSISTENT_DATA_DEBUG
        public const string PlayerFirsts = "Circum_Debug_PlayerFirsts";
        public const string CircumOptions = "Circum_Debug_Options";
        public const string PlayerStats = "Circum_Debug_PlayerStatistics";
        public static string LevelStats(string levelName) => $"Circum_Debug_PlayerStats_{levelName}";
#else
        public const string PlayerFirsts = "Circum_PlayerFirsts";
        public const string CircumOptions = "Circum_Options";
        public const string PlayerStats = "Circum_PlayerStatistics";
        public static string LevelStats(string levelName) => $"Circum_PlayerStats_{levelName}";
#endif
    }
}