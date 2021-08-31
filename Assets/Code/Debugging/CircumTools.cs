using System.Collections.Generic;
using System.Linq;
using Code.Level.Player;
using UnityEditor;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Debugging
{
    public static class CircumTools
    {
#if UNITY_EDITOR
        [MenuItem("Circum/Reset saved player stats")]
        public static void ResetSavedPlayerStats()
        {
            PlayerStats.ResetSavedPlayerStats();
        }
        
        [MenuItem("Circum/Set starter player stats")]
        public static void SetStarterPlayerStats()
        {
            PlayerStats.SetStarterPlayerStats();
        }
        
        [MenuItem("Circum/Set perfect player stats")]
        public static void SetPerfectPlayerStats()
        {
            PlayerStats.SetPerfectPlayerStats();
        }
        
        [MenuItem("Circum/Run validation")]
        public static void RunValidation()
        {
            IEnumerable<IValidateable> validateables = Object.FindObjectsOfType<MonoBehaviour>().OfType<IValidateable>();
            validateables.ApplyFunction(v => v.Validate());
            CircumDebug.Log("--- Validation complete ---");
        }
#endif
    }
}