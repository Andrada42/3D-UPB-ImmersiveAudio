using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    public InputActionReference move3D;
    public InputActionReference look;

    public float speed = 8f;
    public float sensitivity = 0.4f;

    private float _yaw;
    private float _pitch; // si _roll daca voiam;

    private void OnEnable()
    {
        move3D.action.Enable();
        look.action.Enable();
    }

    private void OnDisable()
    {
        move3D.action.Disable();
        look.action.Disable();
    }


    //void Start()
    //{
        
    //}

    private void Update()
    {
        var moveValue = move3D.action.ReadValue<Vector3>();
        var lookValue = look.action.ReadValue<Vector2>();
        // Debug.Log(moveValue);

        _yaw += lookValue.x * sensitivity;
        _pitch -= lookValue.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, -89, 89);

        transform.Translate(moveValue * speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0);
    }
}
