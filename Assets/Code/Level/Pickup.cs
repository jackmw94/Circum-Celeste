using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType _pickupType;

    private int _orbiterLayer;
    private int _playerLayer;

    public static Action PickupCollected = () => { };

    private void Awake()
    {
        _orbiterLayer = LayerMask.NameToLayer($"Orbiter");
        _playerLayer = LayerMask.NameToLayer($"Player");
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"Collided with {other.gameObject}");
        HandleCollision(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered from {other.gameObject}");
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        int otherObjectLayer = other.layer;
        bool collectedByOrbiter = otherObjectLayer == _orbiterLayer && _pickupType.HasFlag(PickupType.OrbiterCollect);
        bool collectedByPlayer = otherObjectLayer == _playerLayer && _pickupType.HasFlag(PickupType.PlayerCollect);
        
        if (collectedByOrbiter || collectedByPlayer)
        {
            PickupCollected();
            Destroy(gameObject);
        }
    }
}