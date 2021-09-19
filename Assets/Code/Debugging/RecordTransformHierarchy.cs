using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Code.Debugging
{
    public class RecordTransformHierarchy : MonoBehaviour
    {
        [SerializeField] private AnimationClip _clip;

#if UNITY_EDITOR
        private GameObjectRecorder _recorder;

        private void Awake()
        {
            _recorder = new GameObjectRecorder(gameObject);
            _recorder.BindComponentsOfType<Transform>(gameObject, true);
        }

        private void OnDestroy()
        {
            if (!_clip)
            {
                return;
            }

            if (_recorder.isRecording)
            {
                _recorder.SaveToClip(_clip);
            }
        }

        private void FixedUpdate()
        {
            if (!_clip)
            {
                return;
            }
            
            _recorder.TakeSnapshot(Time.deltaTime);
        }
#endif
    }
}