using UnityEngine;

namespace Code.Behaviours
{
    public abstract class MaterialProvider : MonoBehaviour
    {
        public abstract Material GetMaterial();
    }
}