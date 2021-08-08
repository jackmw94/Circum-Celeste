using UnityEngine;

public class PlayerOrbiterCollision : MonoBehaviour
{
    private int _orbiterLayer;
    private void Awake()
    {
        _orbiterLayer = LayerMask.NameToLayer($"Orbiter");
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _orbiterLayer)
        {
            
        }
    }

    private void CollisionOccured()
    {
        
    }
}