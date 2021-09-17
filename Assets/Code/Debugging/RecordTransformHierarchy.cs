using UnityEditor.Animations;
using UnityEngine;

namespace Code.Debugging
{
    public class RecordTransformHierarchy : MonoBehaviour
    {
        [SerializeField] private AnimationClip _clip;

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

        private void LateUpdate()
        {
            if (!_clip)
            {
                return;
            }
            
            _recorder.TakeSnapshot(Time.deltaTime);
        }
    }
}