using UnityEngine;
using Random = UnityEngine.Random;

public class PickupGenerator : MonoBehaviour
{
    [SerializeField] private Transform _spawnRoot;
    [Space(15)]
    [SerializeField] private float _chanceOfOrbiter = 0.8f;
    [SerializeField] private GameObject _orbiterPickupPrefab;
    [SerializeField] private GameObject _playerPickupPrefab;

    private void Awake()
    {
        SpawnNewPickup();
    }
    
    private void SpawnNewPickup()
    {
        GameObject pickupPrefab = Random.Range(0f, 1f) < _chanceOfOrbiter ? _orbiterPickupPrefab : _playerPickupPrefab;
        GameObject newPickup = Instantiate(pickupPrefab, _spawnRoot);
        newPickup.transform.localPosition = Vector3.zero;
    }
}