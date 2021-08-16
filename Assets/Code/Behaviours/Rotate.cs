using UnityEngine;

namespace Code.Behaviours
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Vector3 _axes;

        private void Update()
        {
            transform.Rotate(_axes, _speed * Time.deltaTime);
        }
    }
}