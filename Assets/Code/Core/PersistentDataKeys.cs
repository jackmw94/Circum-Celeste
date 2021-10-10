using System;

namespace Code.Core
{
    public static class PersistentDataKeys
    {
#if PERSISTENT_DATA_DEBUG
        public const string SplashScreenLastRunTime = "Circum_Debug_SplashScreenLastRunTime";
        public const string PlayerFirsts = "Circum_Debug_PlayerFirsts";
        public const string CircumOptions = "Circum_Debug_Options";
        public const string PlayerStats = "Circum_Debug_PlayerStatistics";

        [Obsolete] public static string LevelStats_Old(string levelName) => $"Circum_Debug_PlayerStats_{levelName}";
        [Obsolete] public static string PerfectLevelRecording(string levelName) => $"Circum_Debug_LevelRecording_{levelName}";
        [Obsolete] public static string ImperfectLevelRecording(string levelName) => $"Circum_Debug_LevelRecording_{levelName}";
        public static string LevelMetaStats(string levelName) => $"Circum_Debug_LevelStats_{levelName}";
        public static string LevelRecording(string levelName) => $"Circum_Debug_LevelRecording_{levelName}";
#else
        public const string SplashScreenLastRunTime = "Circum_SplashScreenLastRunTime";
        public const string PlayerFirsts = "Circum_PlayerFirsts";
        public const string CircumOptions = "Circum_Options";
        public const string PlayerStats = "Circum_PlayerStatistics";
        
        [Obsolete] public static string LevelStats_Old(string levelName) => $"Circum_PlayerStats_{levelName}";
        [Obsolete] public static string PerfectLevelRecording(string levelName) => $"Circum_PerfectLevelRecording_{levelName}";
        [Obsolete] public static string ImperfectLevelRecording(string levelName) => $"Circum_LevelRecording_{levelName}";
        public static string LevelMetaStats(string levelName) => $"Circum_LevelStats_{levelName}";
        public static string LevelRecording(string levelName) => $"Circum_LevelRecording_{levelName}";
#endif
    }
}