using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Code.Debugging
{
    /// <summary>
    /// Wrapper around logs to allow us to turn them off for release builds
    /// 
    /// I don't want to worry about logs or assertion predicates eating up milliseconds
    /// so need them to be fully compiled out in final versions
    /// 
    /// I'm concerned that Debug.logger.logEnabled = false will still process logs and
    /// assertion predicates, so this seems safer to me
    /// </summary>
    public static class CircumDebug
    {
        [Conditional("CIRCUM_LOGGING")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        [Conditional("CIRCUM_LOGGING")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [Conditional("CIRCUM_LOGGING")]
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }

        [Conditional("CIRCUM_LOGGING")]
        public static void Assert(Func<bool> predicate, string assertionFailMessage)
        {
            Debug.Assert(predicate(), assertionFailMessage);
        }
        
        [Conditional("CIRCUM_LOGGING")]
        public static void Assert(bool assertValue, string assertionFailMessage)
        {
            Debug.Assert(assertValue, assertionFailMessage);
        }
    }
}