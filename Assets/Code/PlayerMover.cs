using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [Space(15)]
    [SerializeField] private float _speed;
    [SerializeField] private bool _useMouse = false;
    
    private void Update()
    {
        _rigidbody.velocity = Vector3.zero;
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _useMouse = !_useMouse;
            Cursor.lockState = _useMouse ? CursorLockMode.Locked : CursorLockMode.None;
        }

        Vector3 movement = _useMouse ? GetMouseMovement() : GetKeyboardMovement();
        
        transform.position += movement * (Time.deltaTime * _speed);
    }

    private Vector3 GetKeyboardMovement()
    {
        Vector3 movement = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) movement += Vector3.up;
        if (Input.GetKey(KeyCode.S)) movement += Vector3.down;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;

        return movement;
    }

    private Vector3 GetMouseMovement()
    {
        //float xAxis = Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f);
        //float yAxis = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f);
        float xAxis = Input.GetAxis("Mouse X");
        float yAxis = Input.GetAxis("Mouse Y");
        return new Vector3(xAxis, yAxis);
    }
}