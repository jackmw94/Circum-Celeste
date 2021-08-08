using System;
using UnityEngine;

public class SlingController : MonoBehaviour
{
    [SerializeField] private float _integralIncrease = 0.2f;
    [SerializeField] private float _warmUpTime = 0.1f;
    [SerializeField] private float _warmDownTime = 0.5f;
    [SerializeField] private OrbitMover _orbitMover;

    private float _currentIncreaseLerpValue = 0f;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _currentIncreaseLerpValue += Time.deltaTime / _warmUpTime;
        }
        else
        {
            _currentIncreaseLerpValue -= Time.deltaTime / _warmDownTime;
        }

        _currentIncreaseLerpValue = Mathf.Clamp01(_currentIncreaseLerpValue);

        float currentIncrease = Mathf.Lerp(0f, _integralIncrease, _currentIncreaseLerpValue);
        
        _orbitMover.SetSlingIncrease(currentIncrease);
    }
}