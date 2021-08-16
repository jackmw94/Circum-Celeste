using UnityEngine;

namespace Code.Behaviours
{
    public class RandomlyDisperseChildren : MonoBehaviour
    {
        [SerializeField] private float _xDistance;
        [SerializeField] private float _yDistance;
    
        [ContextMenu(nameof(Run))]
        private void Run()
        {
            foreach (Transform child in transform)
            {
                float xPos = Random.Range(-_xDistance, _xDistance);
                float yPos = Random.Range(-_yDistance, _yDistance);
                child.localPosition = new Vector3(xPos, yPos, 0f);
            }
        }
    }
}
