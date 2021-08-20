using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Code.VFX
{
    public class DirectionalVfx : MonoBehaviour
    {
        [SerializeField] private VisualEffect _visualEffect;
        [SerializeField] private string _forceDirectionPropertyName = "ForceDirection";

        public void Initialise(Vector2 direction)
        {
            _visualEffect.SetVector2(_forceDirectionPropertyName, direction);
        }
    }
}