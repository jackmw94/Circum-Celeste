#if UNITY_EDITOR

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Code.Debugging
{
    public class CircumBuildValidation : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            CircumTools.RunValidation();
        }
    }
}

#endif