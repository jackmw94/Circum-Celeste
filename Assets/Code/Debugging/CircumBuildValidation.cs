#if UNITY_EDITOR

using Code.Level;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Code.Debugging
{
    public class CircumBuildValidation : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            LevelManager levelManager = Object.FindObjectOfType<LevelManager>();
            if (!levelManager)
            {
                Debug.LogWarning("There is no level manager in scene! I hope you know what you're doing");
                return;
            }

            LevelManager.InputType[] playerInputs = levelManager.PlayersInputs;
            Debug.Assert(playerInputs.Length == 1 && playerInputs[0] == LevelManager.InputType.UI, 
                $"We're not building with expected player inputs! There are {playerInputs.Length}{(playerInputs.Length > 0 ? $" and the first is {playerInputs[0]}" : "")}");
        }
    }
}

#endif