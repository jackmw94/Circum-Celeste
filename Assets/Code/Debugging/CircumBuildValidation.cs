#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityExtras.Code.Core;

namespace Code.Debugging
{
    public class CircumBuildValidation : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            IEnumerable<IValidateable> validateables = Object.FindObjectsOfType<MonoBehaviour>().OfType<IValidateable>();
            validateables.ApplyFunction(v => v.Validate());
        }
    }
}

#endif