using UnityEngine;

namespace Code.Core
{
    [CreateAssetMenu(menuName = "Create GameplayTuning", fileName = "GameplayTuning", order = 0)]
    public class GameplayTuning : ScriptableObject
    {
        [SerializeField] private float _pickupColliderSize = 0.4f;

        public float PickupColliderSize => _pickupColliderSize;
    }
}