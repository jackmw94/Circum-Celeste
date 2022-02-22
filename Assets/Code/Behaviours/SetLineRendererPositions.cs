using UnityEngine;

namespace Code.Behaviours
{
    public class SetLineRendererPositions : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform[] _transforms;

        private void OnEnable()
        {
            _lineRenderer.enabled = true;
        }

        private void OnDisable()
        {
            _lineRenderer.enabled = false;
        }

        private void Update()
        {
            UpdateLineRendererPositions();
        }

        [ContextMenu(nameof(UpdateLineRendererPositions))]
        private void UpdateLineRendererPositions()
        {
            for (int index = 0; index < _transforms.Length; index++)
            {
                Vector3 position = _transforms[index].position;
                _lineRenderer.SetPosition(index, position);
            }
        }
    }
}