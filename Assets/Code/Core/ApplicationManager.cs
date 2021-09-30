using UnityEngine;

namespace Code.Core
{
    public class ApplicationManager : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private bool _vsyncOn;
        
        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
            QualitySettings.vSyncCount = _vsyncOn ? 1 : 0;
        }
    }
}