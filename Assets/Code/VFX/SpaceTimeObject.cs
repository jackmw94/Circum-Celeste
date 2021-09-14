using UnityEngine;

namespace Code.VFX
{
    public class SpaceTimeObject : MonoBehaviour
    {
        private void Awake()
        {
            SpaceTimeHandler.Instance.RegisterObject(gameObject);
        }

        private void OnDestroy()
        {
            SpaceTimeHandler.Instance.UnregisterObject(gameObject);
        }
    }
}