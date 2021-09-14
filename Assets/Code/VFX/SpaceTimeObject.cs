using UnityEngine;

namespace Code.VFX
{
    public class SpaceTimeObject : MonoBehaviour
    {
        private void OnEnable()
        {
            SpaceTimeHandler.Instance.RegisterObject(gameObject);
        }

        private void OnDisable()
        {
            SpaceTimeHandler.Instance.UnregisterObject(gameObject);
        }
    }
}